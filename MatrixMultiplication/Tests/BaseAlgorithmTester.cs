using System;
using BenchmarkDotNet.Attributes;

namespace MatrixMultiplication.Tests
{
	public class BaseAlgorithmTester
	{
		[Params(10, 100, 1000, 10000)]
        public int MatrixSize { get; set; }

		private MatrixProcessor? baseProcessor;
		private int[,]? firstMatrix;
        private int[,]? secondMatrix;

		[IterationSetup]
		public void ResetMatricesAndProcessor()
		{
            firstMatrix = MatrixHelper.GenerateMatrix(MatrixSize, MatrixSize, null);
            secondMatrix = MatrixHelper.GenerateMatrix(MatrixSize, MatrixSize, null);
            baseProcessor = new MatrixProcessor(firstMatrix, secondMatrix);
        }

		[Benchmark]
		public int[,]? TestBase()
		{
			return baseProcessor?.MultiplyMatrices(MultiplicationMethod.Base);
		}

		[Benchmark]
		public int[,]? TestBaseParallel()
		{
			return baseProcessor?.MultiplyMatrices(MultiplicationMethod.BaseParallel);
		}
	}
}

