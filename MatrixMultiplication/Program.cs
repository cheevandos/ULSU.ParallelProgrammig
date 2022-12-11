using BenchmarkDotNet.Running;
using MatrixMultiplication;
using MatrixMultiplication.Tests;
using System.Diagnostics;

//_ = BenchmarkRunner.Run<BaseAlgorithmTester>();

BaseAlgorithmTester test = new();
Console.WriteLine("| Размер матрицы\t| Последовательный алгоритм \t| Параллельный алгоритм \t\t\t\t\t|");
Console.WriteLine("| \t\t\t| \t\t\t\t| 2 потока\t | Изменение\t | 4 потока\t | Изменение\t | 8 потоков\t | Изменение\t|");

var testResults = test.Test();


foreach (var result in testResults)
{
    Console.Write($"| {result.Key} \t\t");
    double prevValue = 0;

    foreach (var iteration in result.Value)
    {
        Console.Write($"| Время: {((double)iteration.Value / 1000)}с; ");
        if (prevValue != 0)
        {
            Console.Write($"| Изменение: {((double)iteration.Value / 1000) - prevValue}с");
        }
        else
        {
            Console.Write("| \t\t\t\t|");
        }
        prevValue = (double)iteration.Value / 1000;
    }
}


Console.ReadKey();