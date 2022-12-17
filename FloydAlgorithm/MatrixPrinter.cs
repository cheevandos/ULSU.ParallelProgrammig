namespace FloydAlgorithm
{
    internal class MatrixPrinter : IPrinter<int[,]>
    {
        public void PrintData(int[,] data)
        {
            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    Console.Write($"\t{data[row, col]}");
                }
                Console.WriteLine();
            }
        }
    }
}