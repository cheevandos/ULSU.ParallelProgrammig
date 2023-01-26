using System.Diagnostics;

namespace ULDecomposition
{
    internal class ULDecompositionTester
    {
        private readonly List<int> MatrixSizes = new() { 100, 1000, 1500, 2500 };
        private readonly List<int> ThreadsCount = new() { 2, 4, 8 };

        private ULDecompositionProcessor? ulProcessor;
        private double[,]? matrix;

        public Dictionary<int, Dictionary<int, long>> Test()
        {
            Dictionary<int, Dictionary<int, long>> statistics = new();

            foreach (int matrixSize in MatrixSizes)
            {
                matrix = MatrixHelper.GenerateMatrix(matrixSize, matrixSize, null);
                ulProcessor = new ULDecompositionProcessor(matrix);

                Dictionary<int, long> iterationStatistics = new();

                Stopwatch timer = new();

                timer.Start();
                ulProcessor.DecomposeUL();
                timer.Stop();

                iterationStatistics.Add(1, timer.ElapsedMilliseconds);

                foreach (int threadCount in ThreadsCount)
                {
                    timer.Reset();
                    timer.Start();
                    ulProcessor.DecomposeULParallel(threadCount);
                    timer.Stop();
                    iterationStatistics.Add(threadCount, timer.ElapsedMilliseconds);
                }

                statistics.Add(matrixSize, iterationStatistics);
            }

            return statistics;
        }
    }
}
