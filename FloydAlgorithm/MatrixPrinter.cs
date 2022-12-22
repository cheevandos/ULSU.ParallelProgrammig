namespace FloydAlgorithm
{
    internal class MatrixPrinter : IPrinter<int[,]>
    {
        const int INDEX_OF_LETTER_A = 65;

        public void PrintData(int[,] data)
        {
            Console.Write($"\t");
            for (int col = 0; col < data.GetLength(1); col++)
            {
                char vertexName = Convert.ToChar(INDEX_OF_LETTER_A + col);
                Console.Write($"{vertexName}\t");
            }
            Console.WriteLine("\n");
            for (int row = 0; row < data.GetLength(0); row++)
            {
                char vertexName = Convert.ToChar(INDEX_OF_LETTER_A + row);
                Console.Write($"{vertexName}\t");
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    string cellValue = data[row, col] == int.MaxValue ? "-" : data[row, col].ToString();
                    Console.Write($"{cellValue}\t");
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");
        }
    }
}