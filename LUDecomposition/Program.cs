namespace MatrixDecomposition
{
    class Program
    {
        static double[,] AMatrix = { { 2.0f, 1.0f, 3.0f }, { 4.0f, 2.0f, 1.0f }, { 3.0f, 1.0f, 2.0f } };
        double[,] U;
        double[,] L;

        static void Main(string[] args)
        {
            int rows = AMatrix.GetLength(0);
            int cols = AMatrix.GetLength(1);
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];
            DecomposeLU(AMatrix, L, U);

            Console.WriteLine("L:");
            PrintMatrix(L);
            Console.WriteLine("U:");
            PrintMatrix(U);
        }

        static void DecomposeLU(double[,] A, double[,] L,  double[,] U)
        {

            // вектор для перестановки строк
            int[] vectorRows = new int[A.GetLength(0)];
            // вектор для перестановки столбцов
            int[] vectorCols = new int[A.GetLength(0)];

            for (int k = 0; k < A.GetLength(0) - 1; k++)
            {
                // инициализация векторов порядка
                for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
                {
                    vectorRows[vectorElem] = vectorElem;
                    vectorCols[vectorElem] = vectorElem;
                }

                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElement(k);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, k, maxElemParams.Item1);
                Swap(vectorCols, k, maxElemParams.Item2);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = AMatrix[vectorRows[k], vectorCols[k]];
                for (int col = k; col < AMatrix.GetLength(1); col++)
                {
                    AMatrix[vectorRows[k], vectorCols[col]] /= element;
                }

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                for (int row = k + 1; row < AMatrix.GetLength(0); row++)
                {
                    double firstRowElement = AMatrix[vectorRows[row], vectorCols[0]];

                    L[vectorRows[row], vectorCols[0]] = firstRowElement;

                    for (int col = k; col < AMatrix.GetLength(1); col++)
                    {
                        AMatrix[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * AMatrix[vectorRows[k], vectorCols[col]];
                        U[vectorRows[row], vectorCols[col]] = AMatrix[vectorRows[row], vectorCols[col]];
                    }
                }
            }

            for (int row = 0; row < AMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < AMatrix.GetLength(1); col++)
                {
                    Console.Write($"{U[row, col]}\t");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");

            for (int row = 0; row < L.GetLength(0); row++)
            {
                for (int col = 0; col < L.GetLength(1); col++)
                {
                    Console.Write($"{L[row, col]}\t");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");
        }

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

        static Tuple<int, int> GetMaxElement(int startIndex)
        {
            double maxElement = AMatrix[startIndex, startIndex];
            int rowIndex = startIndex;
            int colIndex = startIndex;
            for (int row = startIndex; row < AMatrix.GetLength(0); row++)
            {
                for (int col = startIndex; col < AMatrix.GetLength(1); col++)
                {
                    if (Math.Abs(AMatrix[row, col]) > Math.Abs(maxElement))
                    {
                        maxElement = AMatrix[row, col];
                        rowIndex = row;
                        colIndex = col;
                    }
                }
            }
            return new Tuple<int, int>(rowIndex, colIndex);
        }

        static void PrintMatrix(double[,] M)
        {
            int n = M.GetLength(0);
            int m = M.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(M[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}