namespace FloydAlgorithm
{
    internal class FloydProcessor
    {
        // Матрица смежности
        private readonly int[,] adjacencyMatrix;
        // Признак того, что путь между вершинами отсутствует
        private const int WAY_NOT_EXISTS = int.MaxValue;

        public FloydProcessor(int[,] matrix)
        {
            adjacencyMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, adjacencyMatrix, matrix.Length);
        }

        /// <summary>
        /// Алгоритм Флойда (последовательный)
        /// </summary>
        /// <returns>Матрица кратчайших расстояний</returns>
        public int[,] ProcessFloydBase()
        {
            int[,] resultMatrix = new int[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];
            Array.Copy(adjacencyMatrix, resultMatrix, adjacencyMatrix.Length);

            // Внешний цикл по количеству вершин
            for (int vertexNum = 0; vertexNum < adjacencyMatrix.GetLength(0); vertexNum++)
            {
                // Цикл по каждой строке
                for (int row = 0; row < adjacencyMatrix.GetLength(0); row++)
                {
                    // Цикл по каждому столбцу
                    for (int col = 0; col < adjacencyMatrix.GetLength(1); col++)
                    {
                        // Если путь существует
                        if (resultMatrix[row, vertexNum] != WAY_NOT_EXISTS && resultMatrix[vertexNum, col] != WAY_NOT_EXISTS)
                        {
                            // Присваиваем минимальное из значений: либо уже существующее значение, либо сумму значений...
                            resultMatrix[row, col] =
                                Math.Min(
                                    resultMatrix[row, col],
                                    resultMatrix[row, vertexNum] + resultMatrix[vertexNum, col]
                                );
                        }
                    }
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// Алгоритм Флойда (параллельный)
        /// </summary>
        /// <param name="threadsCount">Кол-во потоков для параллельной обработки</param>
        /// <returns>Матрица кратчайших расстояний</returns>
        public int[,] ProcessFloydParallel(int threadsCount)
        {
            int[,] resultMatrix = new int[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];
            Array.Copy(adjacencyMatrix, resultMatrix, adjacencyMatrix.Length);


            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = threadsCount
            };

            // Внешний цикл по количеству вершин
            for (int vertexNum = 0; vertexNum < adjacencyMatrix.GetLength(0); vertexNum++)
            {
                // Цикл по каждой строке (выполняем параллельно)
                Parallel.For(0, adjacencyMatrix.GetLength(0), options, (row) =>
                {
                    if (adjacencyMatrix[row, vertexNum] == WAY_NOT_EXISTS)
                    {
                        return;
                    }
                    // Цикл по каждому столбцу
                    for (int col = 0; col < adjacencyMatrix.GetLength(1); col++)
                    {
                        // Присваиваем минимальное из значений: либо уже существующее значение, либо сумму значений...
                        resultMatrix[row, col] =
                                Math.Min(
                                    resultMatrix[row, col],
                                    resultMatrix[row, vertexNum] + resultMatrix[vertexNum, col]
                                );
                    }
                });
            }

            return resultMatrix;
        }
    }
}