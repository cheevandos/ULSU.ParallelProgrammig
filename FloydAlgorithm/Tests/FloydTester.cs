using MatrixMultiplication;
using System.Diagnostics;

namespace FloydAlgorithm.Tests
{
    /// <summary>
    /// Класс для тестирования алгоритма флойда
    /// </summary>
    internal class FloydTester
    {
        /// <summary>
        /// Список размеров матриц для тестирования
        /// </summary>
        private readonly List<int> MatrixSizes = new() { 100, 1000 };
        /// <summary>
        /// Список количества потоков для тестирования параллельного алгоритма
        /// </summary>
        private readonly List<int> ThreadsCount = new() { 2, 4, 8 };

        /// <summary>
        /// Матрица смежности
        /// </summary>
        private int[,]? adjacencyMatrix;
        /// <summary>
        /// Исполнитель алгоритма
        /// </summary>
        private FloydProcessor? floydProcessor;

        /// <summary>
        /// Функция тестирования
        /// </summary>
        /// <returns>
        /// Коллекция с результатами тестов в формате: [ Размер матрицы: [ Кол-во потоков - Время выполнения ] ]
        /// </returns>
        public Dictionary<int, Dictionary<int, long>> Test()
        {
            Dictionary<int, Dictionary<int, long>> statistics = new();

            // Цикл по размерам матриц
            foreach (int matrixSize in MatrixSizes)
            {
                // Генерируем матрицу смежности
                adjacencyMatrix = MatrixHelper.GenerateMatrix(matrixSize, matrixSize, null);
                // Создаем обработчик
                floydProcessor = new(adjacencyMatrix);

                Dictionary<int, long> iterationStatistics = new();

                // Таймер для измерения времени выполнения
                Stopwatch timer = new();

                // Измеряем время работы базового алгоритма
                timer.Start();
                floydProcessor.ProcessFloydBase();
                timer.Stop();

                iterationStatistics.Add(1, timer.ElapsedMilliseconds);

                // Цикл по количеству потоков
                foreach (int threadCount in ThreadsCount)
                {
                    // Сбрасываем таймер
                    timer.Reset();
                    timer.Start();
                    // Измеряем время работы параллельного алгоритма
                    floydProcessor.ProcessFloydParallel(threadCount);
                    timer.Stop();
                    iterationStatistics.Add(threadCount, timer.ElapsedMilliseconds);
                }

                statistics.Add(matrixSize, iterationStatistics);
            }

            return statistics;
        }
    }
}