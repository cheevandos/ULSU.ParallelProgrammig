using ConsoleTables;
using MatrixMultiplication.Tests;

ITester? algTester = null;

algTester = new BlockAlgorithmTester();

//cold start
Console.WriteLine("Running task on cold start...");
Task testTask = Task.Run(algTester.Test);
Console.WriteLine($"Task status: {testTask.Status}");
testTask.Wait();

// main test
Console.WriteLine("Running main test...");
Task<Dictionary<int, Dictionary<int, long>>> mainTask = Task.Run(algTester.Test);
Console.WriteLine($"Task status: {mainTask.Status}");
mainTask.Wait();

// display results
ConsoleTable? consoleTable = new ConsoleTable(
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
    List<string> iterationResults = new List<string>();

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

Console.ReadKey();