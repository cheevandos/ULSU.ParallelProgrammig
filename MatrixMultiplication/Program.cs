using ConsoleTables;
using MatrixMultiplication.Tests;

ITester? algTester = null;

algTester = new BaseAlgorithmTester();

Console.WriteLine("Холодный старт...");
Task testTask = Task.Run(algTester.Test);
Console.WriteLine($"Статус: {testTask.Status}");
testTask.Wait();

// main test
Console.WriteLine("Основной тест...");
Task<Dictionary<int, Dictionary<int, long>>> mainTask = Task.Run(algTester.Test);
Console.WriteLine($"Статус: {mainTask.Status}");
mainTask.Wait();

// display results
ConsoleTable? consoleTable = new ConsoleTable(
    "Размер матрицы",
    "1 поток",
    "2 потока",
    "Разница",
    "4 потока",
    "Разница",
    "8 потоков",
    "Разница"
);

foreach (var result in mainTask.Result)
{
    List<string> iterationResults = new List<string>();

    iterationResults.Add(result.Key.ToString());

    double singleThreadMS = -1;

    foreach (var iteration in result.Value)
    {
        if (iteration.Key == 1)
        {
            singleThreadMS = (double)iteration.Value / 1000;
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000, 4)} сек");
        }
        else
        {
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000, 4)} сек");
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000 - singleThreadMS, 4)} сек");
        }
    }

    consoleTable.AddRow(iterationResults.ToArray());
}

consoleTable.Write();

Console.ReadKey();