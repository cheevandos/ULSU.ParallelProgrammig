using BenchmarkDotNet.Running;
using MatrixMultiplication.Tests;

_ = BenchmarkRunner.Run<BaseAlgorithmTester>();

Console.ReadKey();