//Алгоритм Флойда
using ConsoleTables;
using FloydAlgorithm;
using FloydAlgorithm.Tests;
using MatrixMultiplication;

// Количество вершин графа
int countOfVertices = 6;

// Генератор случайных чисел для заполнения матрицы смежности
Random rnd = new();

// Матрица смежности для тестирования корректности расчетов
#region Тестирование корректности расчетов
int[,] adjacencyMatrix = MatrixHelper.GenerateMatrix(countOfVertices, null);

IPrinter<int[,]> matrixPrinter = new MatrixPrinter();

Console.WriteLine("Adjacency matrix:");
matrixPrinter.PrintData(adjacencyMatrix);

FloydProcessor floydProcessor = new(adjacencyMatrix);

int[,] floydBaseProcessedMatrix = floydProcessor.ProcessFloydBase();
int[,] floydParallelProcessedMatrix = floydProcessor.ProcessFloydParallel(4);

Console.WriteLine("Base algorithm:");
matrixPrinter.PrintData(floydBaseProcessedMatrix);
Console.WriteLine("Parallel algorithm:");
matrixPrinter.PrintData(floydParallelProcessedMatrix);
#endregion

#region Тестирование на больших объемах данных
FloydTester floydTester = new();

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
#endregion