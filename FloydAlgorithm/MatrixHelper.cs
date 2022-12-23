namespace MatrixMultiplication
{
    public static class MatrixHelper
    {
        public static int[,] GenerateMatrix(
            int countOfVertices,
            Func<int>? numberGenerator
        )
        {
            Func<int> generator = numberGenerator ?? GenerateNumber;

            // Генератор случайных чисел для заполнения матрицы смежности
            Random rnd = new();

            // Матрица смежности
            int[,] adjacencyMatrix = new int[countOfVertices, countOfVertices];

            // Заполнение матрицы смежности
            for (int row = 0; row < adjacencyMatrix.GetLength(0); row++)
            {
                for (int col = row; col < adjacencyMatrix.GetLength(1); col++)
                {
                    if (col == row)
                    {
                        adjacencyMatrix[row, col] = 0;
                    }
                    else
                    {
                        // Флаг существования пути между вершинами
                        bool wayExists = rnd.Next(0, 10) > 1;

                        if (wayExists) // Если путь существует
                        {
                            int randomWeight = generator.Invoke();
                            // Заполняем две связанные ячейки матрицы, считаем, что граф неориентированный
                            adjacencyMatrix[row, col] = randomWeight;
                            adjacencyMatrix[col, row] = randomWeight;
                        }
                        else // Если пути не существует
                        {
                            // Заполняем значением
                            adjacencyMatrix[row, col] = int.MaxValue;
                            adjacencyMatrix[col, row] = int.MaxValue;
                        }
                    }
                }
            }
            return adjacencyMatrix;
        }

        private static int GenerateNumber()
        {
            return new Random().Next(5, 20);
        }
    }
}