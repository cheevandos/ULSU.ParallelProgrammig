namespace ULDecomposition
{
    public static class MatrixHelper
    {
        public static double[,] GenerateMatrix(
            int rows,
            int columns,
            Func<int>? numberGenerator
        )
        {
            Func<int> generator = numberGenerator ?? GenerateNumber;

            double[,] matrix = new double[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    matrix[row, col] = generator.Invoke();
                }
            }

            return matrix;
        }

        private static int GenerateNumber()
        {
            return new Random().Next(1, 10);
        }
    }
}