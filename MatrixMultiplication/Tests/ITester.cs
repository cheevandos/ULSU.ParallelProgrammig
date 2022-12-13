using System;
namespace MatrixMultiplication.Tests
{
    public interface ITester
    {
        Dictionary<int, Dictionary<int, long>> Test();
    }
}