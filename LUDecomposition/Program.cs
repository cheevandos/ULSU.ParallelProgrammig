namespace MatrixDecomposition
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test the matrix decomposition function
            double[,] AMatrix = { { 2.0f, 1.0f, 3.0f }, { 4.0f, 2.0f, 1.0f }, { 3.0f, 1.0f, 2.0f } };
            int rows = AMatrix.GetLength(0);
            int cols = AMatrix.GetLength(1);
            double[,] U = new double[rows, cols];
            double[,] L = new double[rows, cols];
            DecomposeLU(AMatrix, L, U);

            Console.WriteLine("L:");
            PrintMatrix(L);
            Console.WriteLine("U:");
            PrintMatrix(U);
        }

        static void DecomposeLU(double[,] A, double[,] L,  double[,] U)
        {
            int n = A.GetLength(0);

            for (int k = 0; k < n; k++)
            {
                // Find pivot for column
                int pivot = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(A[i, k]) > Math.Abs(A[pivot, k]))
                    {
                        pivot = i;
                    }
                }

                // Swap rows
                if (pivot != k)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double temp = A[k, j];
                        A[k, j] = A[pivot, j];
                        A[pivot, j] = temp;
                    }
                }

                // Compute multipliers
                for (int i = k + 1; i < n; i++)
                {
                    A[i, k] /= A[k, k];
                    for (int j = k + 1; j < n; j++)
                    {
                        A[i, j] -= A[i, k] * A[k, j];
                    }
                }
            }

            // Copy result to L and U matrices
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i > j)
                    {
                        L[i, j] = A[i, j];
                    }
                    else if (i == j)
                    {
                        L[i, j] = 1;
                        U[i, j] = A[i, j];
                    }
                    else
                    {
                        U[i, j] = A[i, j];
                    }
                }
            }
        }

        static void PrintMatrix(double[,] M)
        {
            int n = M.GetLength(0);
            int m = M.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(M[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}