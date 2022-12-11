using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace MatrixMultiplication.Tests
{
    public class BaseAlgorithmTester
    {
        private readonly List<int> MatrixSizes = new() { 100 };
        private readonly List<int> ThreadsCount = new() { 2, 4, 8 };

        private MatrixProcessor? baseProcessor;
        private int[,]? firstMatrix;
        private int[,]? secondMatrix;

        public Dictionary<int, Dictionary<int, long>> Test()
        {
            Dictionary<int, Dictionary<int, long>> statistics = new();

            foreach (int matrixSize in MatrixSizes)
            {
                firstMatrix = MatrixHelper.GenerateMatrix(matrixSize, matrixSize, null);
                secondMatrix = MatrixHelper.GenerateMatrix(matrixSize, matrixSize, null);
                baseProcessor = new MatrixProcessor(firstMatrix, secondMatrix);

                Dictionary<int, long> iterationStatistics = new();

                Stopwatch timer = new();

                timer.Start();
                baseProcessor.MultiplyMatricesBase();
                timer.Stop();

                iterationStatistics.Add(1, timer.ElapsedMilliseconds);

                foreach (int threadCount in ThreadsCount)
                {
                    timer.Reset();
                    timer.Start();
                    baseProcessor.MultiplyMatricesBaseParallel(threadCount);
                    timer.Stop();
                    iterationStatistics.Add(threadCount, timer.ElapsedMilliseconds);
                }

                statistics.Add(matrixSize, iterationStatistics);
            }

            return statistics;
        }
    }
}