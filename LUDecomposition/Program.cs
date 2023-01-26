using ConsoleTables;
using LUDecomposition;

double[,] AMatrix =
{
    { 2.0f, 1.0f, 3.0f },
    { 4.0f, 2.0f, 1.0f },
    { 3.0f, 1.0f, 2.0f }
};

Console.WriteLine("Тестовая матрица 3х3:");
LUDecompositionProcessor LUDecompositionProcessor = new(MatrixHelper.GenerateMatrix(3, 3, null), true);
//LUDecompositionProcessor LUDecompositionProcessor = new(AMatrix, true);
Console.WriteLine("Однопоточный метод:");
LUDecompositionProcessor.DecomposeLU();
Console.WriteLine("Многопоточный метод (2 потока):");
LUDecompositionProcessor.DecomposeLUParallel(2);

LUDecompositionTester ulTester = new();

// Основной запуск алгоритма
Console.WriteLine("Running main test...");
Task<Dictionary<int, Dictionary<int, long>>> mainTask = Task.Run(ulTester.Test);
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