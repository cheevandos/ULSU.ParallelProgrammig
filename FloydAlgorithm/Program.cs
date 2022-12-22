//Алгоритм Флойда
using ConsoleTables;
using FloydAlgorithm;
using FloydAlgorithm.Tests;

// Количество вершин графа
int countOfVertices = 6;

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
            bool wayExists = rnd.Next(0, 10) > 3;

            if (wayExists) // Если путь существует
            {
                int randomWeight = (int)rnd.Next(2, 20);
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

IPrinter<int[,]> matrixPrinter = new MatrixPrinter();

Console.WriteLine("Adjacency matrix:");
matrixPrinter.PrintData(adjacencyMatrix);

FloydProcessor floydProcessor = new(adjacencyMatrix);

int[,] floydBaseProcessedMatrix = floydProcessor.ProcessFloydBase();
int[,] floydParallelProcessedMatrix = floydProcessor.ProcessFloydBase();

Console.WriteLine("Base algorithm:");
matrixPrinter.PrintData(floydBaseProcessedMatrix);
Console.WriteLine("Parallel algorithm:");
matrixPrinter.PrintData(floydParallelProcessedMatrix);

FloydTester floydTester = new();

// Первый запуск алгоритма (холодный старт)
Console.WriteLine("Running task on cold start...");
Task testTask = Task.Run(floydTester.Test);
Console.WriteLine($"Task status: {testTask.Status}");
testTask.Wait();

// Основной запуск алгоритма
Console.WriteLine("Running main test...");
Task<Dictionary<int, Dictionary<int, long>>> mainTask = Task.Run(floydTester.Test);
Console.WriteLine($"Task status: {mainTask.Status}");
mainTask.Wait();

// Вывод результатов в виде сравнительной таблицы
ConsoleTable? consoleTable = new(
    "Matrix size",
    "1 thread",
    "2 threads",
    "Diff",
    "4 threads",
    "Diff",
    "8 threads",
    "Diff"
);

foreach (var result in mainTask.Result)
{
    List<string> iterationResults = new();

    iterationResults.Add(result.Key.ToString());

    double singleThreadMS = -1;

    foreach (var iteration in result.Value)
    {
        if (iteration.Key == 1)
        {
            singleThreadMS = (double)iteration.Value / 1000;
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000, 4)} sec");
        }
        else
        {
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000, 4)} sec");
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000 - singleThreadMS, 4)} sec");
        }
    }

    consoleTable.AddRow(iterationResults.ToArray());
}

consoleTable.Write();