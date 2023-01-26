using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUDecomposition
{
    internal class LUDecompositionProcessor
    {
        // вектор для перестановки строк
        static int[]? vectorRows;
        // вектор для перестановки столбцов
        static int[]? vectorCols;

        static int rows;
        static int cols;
        readonly double[,] localMatrixForMultiThread;
        readonly double[,] localMatrixForSingleThread;

        readonly bool needPrint;

        public LUDecompositionProcessor(double[,] matrix, bool needPrint = false)
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

        public void DecomposeLU()
        {
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];

            vectorRows = new int[rows];
            vectorCols = new int[cols];

            // инициализация векторов порядка
            for (int vectorElem = 0; vectorElem < vectorRows.Length; vectorElem++)
            {
                vectorRows[vectorElem] = vectorElem;
                vectorCols[vectorElem] = vectorElem;
            }

            for (int k = 0; k < rows; k++)
            {
                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElement(k, localMatrixForSingleThread);

                // изменяем порядок столбцов/строк
                Swap(vectorRows, k, maxElemParams.Item1);
                Swap(vectorCols, k, maxElemParams.Item2);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = localMatrixForSingleThread[vectorRows[k], vectorCols[k]];
                L[vectorRows[k], vectorCols[k]] = element;
                for (int col = k; col < localMatrixForSingleThread.GetLength(1); col++)
                {
                    localMatrixForSingleThread[vectorRows[k], vectorCols[col]] /= element;
                    U[vectorRows[k], vectorCols[col]] = localMatrixForSingleThread[vectorRows[k], vectorCols[col]];
                }

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                for (int row = k + 1; row < localMatrixForSingleThread.GetLength(0); row++)
                {
                    double firstRowElement = localMatrixForSingleThread[vectorRows[row], vectorCols[k]];

                    L[vectorRows[row], vectorCols[k]] = firstRowElement;

                    for (int col = k; col < localMatrixForSingleThread.GetLength(1); col++)
                    {
                        localMatrixForSingleThread[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * localMatrixForSingleThread[vectorRows[k], vectorCols[col]];
                        U[vectorRows[row], vectorCols[col]] = localMatrixForSingleThread[vectorRows[row], vectorCols[col]];
                    }
                }
            }

            if (needPrint)
            {
                PrintMatrix("U", U);
                PrintMatrix("L", L);
            }
        }

        public void DecomposeLUParallel(int threadsCount)
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

            for (int k = 0; k < localMatrixForMultiThread.GetLength(0); k++)
            {
                // получаем строку и столбец максимального элемента
                Tuple<int, int> maxElemParams = GetMaxElementParallel(k, threadsCount, localMatrixForMultiThread);

                // изменяем порядок столбцов/строк 
                Swap(vectorRows, k, maxElemParams.Item1);
                Swap(vectorCols, k, maxElemParams.Item2);

                // Получаем первый элемент и делим на этот элемент всю строку
                double element = localMatrixForMultiThread[vectorRows[k], vectorCols[k]];
                L[vectorRows[k], vectorCols[k]] = element;

                ParallelOptions options = new()
                {
                    MaxDegreeOfParallelism = threadsCount
                };

                Parallel.For(k, localMatrixForMultiThread.GetLength(1), options, (col) =>
                {
                    localMatrixForMultiThread[vectorRows[k], vectorCols[col]] /= element;
                    U[vectorRows[k], vectorCols[col]] = localMatrixForMultiThread[vectorRows[k], vectorCols[col]];
                });

                // Вычитаем из каждой последующей строки первую строку подматрицы,
                // умноженную на первый элемент
                Parallel.For(k + 1, localMatrixForMultiThread.GetLength(0), options, (row) =>
                {
                    double firstRowElement = localMatrixForMultiThread[vectorRows[row], vectorCols[k]];

                    L[vectorRows[row], vectorCols[k]] = firstRowElement;

                    for (int col = k; col < localMatrixForMultiThread.GetLength(1); col++)
                    {
                        localMatrixForMultiThread[vectorRows[row], vectorCols[col]] -=
                            firstRowElement * localMatrixForMultiThread[vectorRows[k], vectorCols[col]];
                        U[vectorRows[row], vectorCols[col]] = localMatrixForMultiThread[vectorRows[row], vectorCols[col]];
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

        static Tuple<int, int> GetMaxElementParallel(int startIndex, int threadsCount, double[,] matrix)
        {
            int rowIndex = startIndex;
            int colIndex = startIndex;
            double maxElement = matrix[vectorRows[rowIndex], vectorCols[colIndex]];

            ParallelOptions options = new()
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
#nullable enable
    }
}