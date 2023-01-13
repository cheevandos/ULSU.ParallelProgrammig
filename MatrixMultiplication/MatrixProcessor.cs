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

        public int[,] MultiplyMatricesBlock()
        {
            resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            int gridSize = (int)(Math.Sqrt(firstMatrix.GetLength(0)));

            while (firstMatrix.GetLength(0) % gridSize != 0)
            {
                gridSize++;
            }

            int blockSize = firstMatrix.GetLength(0) / gridSize;
            int blocksCount = gridSize * gridSize;

            List<Tuple<int, int, int, int>> blocks = new();

            for (int blockNum = 0; blockNum < blocksCount; blockNum++)
            {
                int startRow = (blockNum / gridSize) * blockSize;
                int endRow = startRow + blockSize;
                int startCol = (blockNum % gridSize) * blockSize;
                int endCol = startCol + blockSize;
                blocks.Add(new Tuple<int, int, int, int>(startRow, endRow, startCol, endCol));
            }

            foreach(Tuple<int, int, int, int> block in blocks)
            {
                CalculateResultMatrixBlock(block);
            }

            return resultMatrix;
        }

        public int[,] MultiplyMatricesBlockParallel(int threadsCount)
        {
            resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

            int gridSize = (int)(Math.Sqrt(firstMatrix.GetLength(0)));

            while (firstMatrix.GetLength(0) % gridSize != 0)
            {
                gridSize++;
            }

            int blockSize = firstMatrix.GetLength(0) / gridSize;
            int blocksCount = gridSize * gridSize;

            List<Tuple<int, int, int, int>> blocks = new();

            for (int blockNum = 0; blockNum < blocksCount; blockNum++)
            {
                int startRow = (blockNum / gridSize) * blockSize;
                int endRow = startRow + blockSize;
                int startCol = (blockNum % gridSize) * blockSize;
                int endCol = startCol + blockSize;
                blocks.Add(new Tuple<int, int, int, int>(startRow, endRow, startCol, endCol));
            }

            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = threadsCount
            };

            Parallel.ForEach(blocks, options, CalculateResultMatrixBlock);

            return resultMatrix;
        }



#nullable disable
        private void CalculateResultMatrixBlock(Tuple<int, int, int, int> block)
        {
            for (int row = block.Item1; row < block.Item2; row++)
            {
                for (int col = 0; col < secondMatrix.GetLength(1); col++)
                {
                    for (int secMatrixRow = block.Item3; secMatrixRow < block.Item4; secMatrixRow++)
                    {
                        Interlocked.Add(
                            ref resultMatrix[row, col],
                            firstMatrix[row, secMatrixRow] *
                            secondMatrix[secMatrixRow, col]
                        );
                    }
                }
            }
        }
#nullable enable
    }
}