using System.Collections.Concurrent;

namespace MatrixMultiplication
{
    public static class MatrixHelper
	{
		public static int[,] GenerateMatrix(
			int rows,
			int columns,
            Func<int>? numberGenerator
		)
		{
            Func<int> generator = numberGenerator ?? GenerateNumber;

			int[,] matrix = new int[rows, columns];

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
            return new Random().Next(10, 99);
        }
    }
}

