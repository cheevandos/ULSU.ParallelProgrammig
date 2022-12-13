using ConsoleTables;
using MatrixMultiplication;
using MatrixMultiplication.Tests;
using System.Diagnostics;

MultiplicationMethod method;
try
{
    method = (MultiplicationMethod)Enum.Parse(typeof(MultiplicationMethod), args[0]);
}
catch (Exception)
{
    Console.WriteLine("Can't recognize multiplication method [Base, BaseParallel, Block, BlockParallel]");
    Environment.Exit(-1);
}

BaseAlgorithmTester baseAlgTester = new();
//cold start
Console.WriteLine("Running task on cold start...");
Task testTask = Task.Run(baseAlgTester.Test);
Console.WriteLine($"Task status: {testTask.Status}");
testTask.Wait();

// main test
Console.Clear();
Console.WriteLine("Running main test...");
Task<Dictionary<int, Dictionary<int, long>>> mainTask = Task.Run(baseAlgTester.Test);
Console.WriteLine($"Task status: {mainTask.Status}");
mainTask.Wait();

// display results
Console.Clear();
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

    double prevValue = 0;

    foreach (var iteration in result.Value)
    {
        iterationResults.Add($"{Math.Round((double)iteration.Value / 1000, 4)} sec");
        if (prevValue != 0)
        {
            iterationResults.Add($"{Math.Round((double)iteration.Value / 1000 - prevValue, 4)} sec");
        }
        prevValue = (double)iteration.Value / 1000;
    }

    consoleTable.AddRow(iterationResults.ToArray());
}

consoleTable.Write();

Console.ReadKey();