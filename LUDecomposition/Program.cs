using ConsoleTables;

namespace ULDecomposition
{
    class Program
    {
        static double[,] AMatrix =
        {
            { 2.0f, 1.0f, 3.0f },
            { 4.0f, 2.0f, 1.0f },
            { 3.0f, 1.0f, 2.0f }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Тестовая матрица 3х3:");
            //ULDecompositionProcessor uLDecompositionProcessor = new(MatrixHelper.GenerateMatrix(3, 3, null), true);
            ULDecompositionProcessor uLDecompositionProcessor = new(AMatrix, true);
            Console.WriteLine("Однопоточный метод:");
            uLDecompositionProcessor.DecomposeUL();
            Console.WriteLine("Многопоточный метод (2 потока):");
            uLDecompositionProcessor.DecomposeULParallel(2);

            ULDecompositionTester ulTester = new();

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
        }
    }
}