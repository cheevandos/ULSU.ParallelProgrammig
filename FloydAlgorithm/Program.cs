
//Алгоритм Флойда

// Количество вершин графа
using FloydAlgorithm;

int countOfVertices = 10;

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
            bool wayExists = rnd.Next(0, 2) > 0;

            if (wayExists) // Если путь существует
            {
                int randomWeight = rnd.Next(5, 50);
                // Заполняем две связанные ячейки матрицы, считаем, что граф неориентированный
                adjacencyMatrix[row, col] = randomWeight;
                adjacencyMatrix[col, row] = randomWeight;
            }
            else // Если пути не существует
            {
                // Заполняем п
                adjacencyMatrix[row, col] = -1;
            }
        }
    }

    IPrinter<int[,]> matrixPrinter = new MatrixPrinter();

    matrixPrinter.PrintData(adjacencyMatrix);
}