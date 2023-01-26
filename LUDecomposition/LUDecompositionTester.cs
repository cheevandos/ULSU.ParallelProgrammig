using System.Diagnostics;

namespace LUDecomposition
{
    internal class LUDecompositionTester
    {
        private readonly List<int> MatrixSizes = new() { 100, 1000, 1500, 2500 };
        private readonly List<int> ThreadsCount = new() { 2, 4, 8 };

        private LUDecompositionProcessor? ulProcessor;
        private double[,]? matrix;

        public Dictionary<int, Dictionary<int, long>> Test()
        {
            Dictionary<int, Dictionary<int, long>> statistics = new();

            foreach (int matrixSize in MatrixSizes)
            {
                matrix = MatrixHelper.GenerateMatrix(matrixSize, matrixSize, null);
                ulProcessor = new LUDecompositionProcessor(matrix);

                Dictionary<int, long> iterationStatistics = new();

                Stopwatch timer = new();

                timer.Start();
                ulProcessor.DecomposeLU();
                timer.Stop();

                iterationStatistics.Add(1, timer.ElapsedMilliseconds);

                foreach (int threadCount in ThreadsCount)
                {
                    timer.Reset();
                    timer.Start();
                    ulProcessor.DecomposeLUParallel(threadCount);
                    timer.Stop();
                    iterationStatistics.Add(threadCount, timer.ElapsedMilliseconds);
                }

                statistics.Add(matrixSize, iterationStatistics);
            }

            return statistics;
        }
    }
}