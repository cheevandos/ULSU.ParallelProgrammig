namespace MatrixDecomposition
{
    class Program
    {
        static double[,] AMatrix =
        {
            { 2.0f, 1.0f, 3.0f },
            { 4.0f, 2.0f, 1.0f },
            { 3.0f, 1.0f, 2.0f }
        };

        // вектор для перестановки строк
        static int[]? vectorRows;
        // вектор для перестановки столбцов
        static int[]? vectorCols;

        static int rows;
        static int cols;

        static void Main(string[] args)
        {
            rows = AMatrix.GetLength(0);
            cols = AMatrix.GetLength(1);

            double[,] MatrixForSignleThread = new double[rows, cols];
            double[,] MatrixForMultiThread = new double[rows, cols];
            Array.Copy(AMatrix, MatrixForSignleThread, AMatrix.Length);
            Array.Copy(AMatrix, MatrixForMultiThread, AMatrix.Length);

            DecomposeLU(MatrixForSignleThread);
            DecomposeLUParallel(MatrixForMultiThread, 4);
        }

        static void DecomposeLU(double[,] matrix)
        {
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];

            vectorRows = new int[matrix.GetLength(0)];
            vectorCols = new int[matrix.GetLength(0)];

            // инициализация векторов порядка
            for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
            {
                vectorRows[vectorElem] = vectorElem;
                vectorCols[vectorElem] = vectorElem;
            }

            for (int k = 0; k < matrix.GetLength(0); k++)
            {
                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElement(k, matrix);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, k, maxElemParams.Item1);
                Swap(vectorCols, k, maxElemParams.Item2);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = matrix[vectorRows[k], vectorCols[k]];
                L[vectorRows[k], vectorCols[k]] = element;
                for (int col = k; col < matrix.GetLength(1); col++)
                {
                    matrix[vectorRows[k], vectorCols[col]] /= element;
                    U[vectorRows[k], vectorCols[col]] = matrix[vectorRows[k], vectorCols[col]];
                }

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                for (int row = k + 1; row < matrix.GetLength(0); row++)
                {
                    double firstRowElement = matrix[vectorRows[row], vectorCols[k]];

                    L[vectorRows[row], vectorCols[k]] = firstRowElement;

                    for (int col = k; col < matrix.GetLength(1); col++)
                    {
                        matrix[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * matrix[vectorRows[k], vectorCols[col]];
                        U[vectorRows[row], vectorCols[col]] = matrix[vectorRows[row], vectorCols[col]];
                    }
                }
            }
            PrintMatrix("U", U);
            PrintMatrix("L", L);
        }

        static void DecomposeLUParallel(double[,] matrix, int threadsCount)
        {
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];

            vectorRows = new int[matrix.GetLength(0)];
            vectorCols = new int[matrix.GetLength(0)];

            // инициализация векторов порядка
            for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
            {
                vectorRows[vectorElem] = vectorElem;
                vectorCols[vectorElem] = vectorElem;
            }

            for (int k = 0; k < matrix.GetLength(0); k++)
            {

                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElementParallel(k, threadsCount, matrix);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, k, maxElemParams.Item1);
                Swap(vectorCols, k, maxElemParams.Item2);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = matrix[vectorRows[k], vectorCols[k]];
                L[vectorRows[k], vectorCols[k]] = element;

                ParallelOptions options = new()
                {
                    MaxDegreeOfParallelism = threadsCount
                };

                Parallel.For(k, matrix.GetLength(1), options, (col) =>
                {
                    matrix[vectorRows[k], vectorCols[col]] /= element;
                    U[vectorRows[k], vectorCols[col]] = matrix[vectorRows[k], vectorCols[col]];
                });

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                Parallel.For(k + 1, matrix.GetLength(0), options, (row) =>
                {
                    double firstRowElement = matrix[vectorRows[row], vectorCols[k]];

                    L[vectorRows[row], vectorCols[k]] = firstRowElement;

                    for (int col = k; col < matrix.GetLength(1); col++)
                    {
                        matrix[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * matrix[vectorRows[k], vectorCols[col]];
                        U[vectorRows[row], vectorCols[col]] = matrix[vectorRows[row], vectorCols[col]];
                    }
                });
            }
            PrintMatrix("U", U);
            PrintMatrix("L", L);
        }

        #nullable disable
        public static void PrintMatrix(string matrixName, double[,] matrix)
        {
            Console.WriteLine($"Матрица {matrixName}:\n");
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Console.Write($"{matrix[vectorRows[row], vectorCols[col]]}\t");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");
        }
        #nullable enable

        /// <summary>
        /// Замена порядка столбцов/строк
        /// </summary>
        /// <param name="vector">Вектор порядка</param>
        /// <param name="index">Индекс строки/столбца</param>
        /// <param name="current">На какой заменить</param>
        static void Swap(int[] vector, int index, int current)
        {
            int temp = vector[index];
            vector[index] = current;
            vector[current] = temp;
        }

        #nullable disable
        static Tuple<int, int> GetMaxElement(int startIndex, double[,] matrix)
        {
            int rowIndex = startIndex;
            int colIndex = startIndex;
            double maxElement = matrix[vectorRows[rowIndex], vectorCols[colIndex]];
            for (int row = startIndex; row < matrix.GetLength(0); row++)
            {
                for (int col = startIndex; col < matrix.GetLength(1); col++)
                {
                    if (Math.Abs(matrix[vectorRows[row], vectorCols[col]]) > Math.Abs(maxElement))
                    {
                        maxElement = matrix[vectorRows[row], vectorCols[col]];
                        rowIndex = row;
                        colIndex = col;
                    }
                }
            }
            return new Tuple<int, int>(rowIndex, colIndex);
        }
        #nullable enable

        static Tuple<int, int> GetMaxElementParallel(int startIndex, int threadsCount, double[,] matrix)
        {
            int rowIndex = startIndex;
            int colIndex = startIndex;
            double maxElement = matrix[vectorRows[rowIndex], vectorCols[colIndex]];

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = threadsCount
            };
            Parallel.For(startIndex, matrix.GetLength(0), options, (row) =>
            {
                for (int col = startIndex; col < matrix.GetLength(1); col++)
                {
                    if (Math.Abs(matrix[vectorRows[row], vectorCols[col]]) > Math.Abs(maxElement))
                    {
                        maxElement = matrix[vectorRows[row], vectorCols[col]];
                        rowIndex = row;
                        colIndex = col;
                    }
                }
            });
            return new Tuple<int, int>(rowIndex, colIndex);
        }
    }
}