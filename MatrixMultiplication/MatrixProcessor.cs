using System;
using System.Collections.Concurrent;

namespace MatrixMultiplication
{
    public class MatrixProcessor
	{
        private readonly int[,] firstMatrix;
        private readonly int[,] secondMatrix;
        private int[,]? resultMatrix;

		public MatrixProcessor(int[,] firstMatrix, int[,] secondMatrix)
		{
            this.firstMatrix = new int[firstMatrix.GetLength(0), firstMatrix.GetLength(1)];
            this.secondMatrix = new int[secondMatrix.GetLength(0), secondMatrix.GetLength(1)];

            Array.Copy(firstMatrix, this.firstMatrix, firstMatrix.Length);
            Array.Copy(secondMatrix, this.secondMatrix, secondMatrix.Length);
		}

		public int[,] MultiplyMatrices(MultiplicationMethod multiplicationMethod)
		{
            if (firstMatrix.GetLength(0) != secondMatrix.GetLength(1))
            {
                throw new ArithmeticException(
                    "Количество столбцов первой матрицы должно " +
                    "совпадать с количеством строк второй матрицы"
                );
            }

			Func<int[,]> multiplicator;

            multiplicator = multiplicationMethod switch
            {
                MultiplicationMethod.Base => MultiplyMatricesBase,
                MultiplicationMethod.BaseParallel => MultiplyMatricesBaseParallel,
                MultiplicationMethod.Tape => throw new NotImplementedException(),
                MultiplicationMethod.TapeParallel => throw new NotImplementedException(),
                MultiplicationMethod.Block => throw new NotImplementedException(),
                MultiplicationMethod.BlockParallel => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };

            return multiplicator.Invoke();
        }

        private int[,] MultiplyMatricesBase()
        {
            resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            for (int firstMatrixRowNumber = 0; firstMatrixRowNumber < firstMatrix.GetLength(0); firstMatrixRowNumber++)
            {
                CalculateResultMatrixRow(firstMatrixRowNumber);
            }

            return resultMatrix;
        }

#nullable disable
        private void CalculateResultMatrixRow(int rowNumber)
        {
            for (int firstMatrixCol = 0; firstMatrixCol < firstMatrix.GetLength(1); firstMatrixCol++)
            {
                for (int secondMatrixRow = 0; secondMatrixRow < secondMatrix.GetLength(0); secondMatrixRow++)
                {
                    resultMatrix[rowNumber, firstMatrixCol] +=
                        firstMatrix[rowNumber, secondMatrixRow] *
                        secondMatrix[secondMatrixRow, firstMatrixCol];
                }
            }
        }
#nullable enable

        private int[,] MultiplyMatricesBaseParallel()
        {
            resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            Parallel.For(0, firstMatrix.GetLength(0), CalculateResultMatrixRow);
            return resultMatrix;
        }
    }
}

