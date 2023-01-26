namespace ULDecomposition
{
    internal class ULDecompositionProcessor
    {
        // вектор для перестановки строк
        static int[]? vectorRows;
        // вектор для перестановки столбцов
        static int[]? vectorCols;

        static int rows;
        static int cols;
        double[,] localMatrixForMultiThread;
        double[,] localMatrixForSingleThread;

        readonly bool needPrint;

        public ULDecompositionProcessor(double[,] matrix, bool needPrint = false)
        {
            rows = matrix.GetLength(0);
            cols = matrix.GetLength(1);
            this.needPrint = needPrint;

            localMatrixForMultiThread = new double[rows, cols];
            Array.Copy(matrix, localMatrixForMultiThread, matrix.Length);

            localMatrixForSingleThread = new double[rows, cols];
            Array.Copy(matrix, localMatrixForSingleThread, matrix.Length);

            if (needPrint)
            {
                vectorRows = new int[matrix.GetLength(0)];
                vectorCols = new int[matrix.GetLength(0)];

                // инициализация векторов порядка
                for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
                {
                    vectorRows[vectorElem] = vectorElem;
                    vectorCols[vectorElem] = vectorElem;
                }
                PrintMatrix("Исходная матрица:", matrix);
            }
        }

        public void DecomposeUL()
        {
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];

            vectorRows = new int[localMatrixForSingleThread.GetLength(0)];
            vectorCols = new int[localMatrixForSingleThread.GetLength(0)];

            // инициализация векторов порядка
            for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
            {
                vectorRows[vectorElem] = vectorElem;
                vectorCols[vectorElem] = vectorElem;
            }
            int revertedIndex = localMatrixForSingleThread.GetLength(0) - 1;

            for (int k = 0; k < localMatrixForSingleThread.GetLength(0); k++)
            {
                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElement(k, localMatrixForSingleThread);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, maxElemParams.Item1, revertedIndex - k);
                Swap(vectorCols, maxElemParams.Item2, revertedIndex - k);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = localMatrixForSingleThread[vectorRows[revertedIndex - k], vectorCols[revertedIndex - k]];
                U[vectorRows[revertedIndex - k], vectorCols[revertedIndex - k]] = element;
                for (int col = revertedIndex - k; col >= 0; col--)
                {
                    localMatrixForSingleThread[vectorRows[revertedIndex - k], vectorCols[col]] /= element;
                    L[vectorRows[revertedIndex - k], vectorCols[col]] =
                        localMatrixForSingleThread[vectorRows[revertedIndex - k], vectorCols[col]];
                }

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                for (int row = revertedIndex - k - 1; row >= 0; row--)
                {
                    double firstRowElement = localMatrixForSingleThread[vectorRows[row], vectorCols[revertedIndex - k]];

                    U[vectorRows[row], vectorCols[revertedIndex - k]] = firstRowElement;

                    for (int col = revertedIndex - k; col >= 0; col--)
                    {
                        localMatrixForSingleThread[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * localMatrixForSingleThread[vectorRows[revertedIndex - k], vectorCols[col]];
                        L[vectorRows[row], vectorCols[col]] = localMatrixForSingleThread[vectorRows[row], vectorCols[col]];
                    }
                }
            }
            if (needPrint)
            {
                PrintMatrix("U", U);
                PrintMatrix("L", L);
            }
        }

        public void DecomposeULParallel(int threadsCount)
        {
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];

            vectorRows = new int[localMatrixForMultiThread.GetLength(0)];
            vectorCols = new int[localMatrixForMultiThread.GetLength(0)];

            // инициализация векторов порядка
            for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
            {
                vectorRows[vectorElem] = vectorElem;
                vectorCols[vectorElem] = vectorElem;
            }
            int revertedIndex = localMatrixForMultiThread.GetLength(0) - 1;

            for (int k = 0; k < localMatrixForMultiThread.GetLength(0); k++)
            {
                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElementParallel(k, threadsCount, localMatrixForMultiThread);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, maxElemParams.Item1, revertedIndex - k);
                Swap(vectorCols, maxElemParams.Item2, revertedIndex - k);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = localMatrixForMultiThread[vectorRows[revertedIndex - k], vectorCols[revertedIndex - k]];
                U[vectorRows[revertedIndex - k], vectorCols[revertedIndex - k]] = element;

                ParallelOptions options = new()
                {
                    MaxDegreeOfParallelism = threadsCount
                };

                Parallel.For(0, revertedIndex - k + 1, options, (col) =>
                {
                    localMatrixForMultiThread[vectorRows[revertedIndex - k], vectorCols[col]] /= element;
                    L[vectorRows[revertedIndex - k], vectorCols[col]] =
                        localMatrixForMultiThread[vectorRows[revertedIndex - k], vectorCols[col]];
                });

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                Parallel.For(0, revertedIndex - k, options, (row) =>
                {
                    double firstRowElement = localMatrixForMultiThread[vectorRows[row], vectorCols[revertedIndex - k]];

                    U[vectorRows[row], vectorCols[revertedIndex - k]] = firstRowElement;

                    for (int col = revertedIndex - k; col >= 0; col--)
                    {
                        localMatrixForMultiThread[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * localMatrixForMultiThread[vectorRows[revertedIndex - k], vectorCols[col]];
                        L[vectorRows[row], vectorCols[col]] = localMatrixForMultiThread[vectorRows[row], vectorCols[col]];
                    }
                });
            }

            if (needPrint)
            {
                PrintMatrix("U", U);
                PrintMatrix("L", L);
            }
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
            vector[index] = vector[current];
            vector[current] = temp;
        }

#nullable disable
        private static Tuple<int, int> GetMaxElement(int startIndex, double[,] matrix)
        {
            int revertedIndex = matrix.GetLength(0) - 1;
            int rowIndex = startIndex;
            int colIndex = startIndex;
            double maxElement = matrix[vectorRows[revertedIndex - rowIndex], vectorCols[revertedIndex - colIndex]];
            for (int row = revertedIndex - startIndex; row >= 0; row--)
            {
                for (int col = revertedIndex - startIndex; col >= 0; col--)
                {
                    if (Math.Abs(matrix[vectorRows[row], vectorCols[col]]) >= Math.Abs(maxElement))
                    {
                        maxElement = matrix[vectorRows[row], vectorCols[col]];
                        rowIndex = row;
                        colIndex = col;
                    }
                }
            }
            return new Tuple<int, int>(rowIndex, colIndex);
        }

        private static Tuple<int, int> GetMaxElementParallel(int startIndex, int threadsCount, double[,] matrix)
        {
            int revertedIndex = matrix.GetLength(0) - 1;
            int rowIndex = startIndex;
            int colIndex = startIndex;
            double maxElement = matrix[vectorRows[revertedIndex - rowIndex], vectorCols[revertedIndex - colIndex]];
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = threadsCount
            };
            Parallel.For(0, revertedIndex - startIndex + 1, options, (row) =>
            {
                for (int col = revertedIndex - startIndex; col >= 0; col--)
                {
                    if (Math.Abs(matrix[vectorRows[row], vectorCols[col]]) >= Math.Abs(maxElement))
                    {
                        maxElement = matrix[vectorRows[row], vectorCols[col]];
                        rowIndex = row;
                        colIndex = col;
                    }
                }
            });
            return new Tuple<int, int>(rowIndex, colIndex);
        }
#nullable enable
    }
}