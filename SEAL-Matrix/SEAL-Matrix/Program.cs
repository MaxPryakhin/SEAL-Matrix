using SEAL_Matrix.Core.Matrix;
using System;
using Microsoft.Research.SEAL;

namespace SEAL_Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrixStrategy = new MatrixStrategy();
            var matrixContext = new MatrixContext(matrixStrategy);
            var matrix = new double[] { 1.0, 0.5, 2.0, 5.0 };
            var rowSize = 2;

            Console.WriteLine("init matrix");
            matrixContext.InitMatrix(matrix, rowSize);

            Console.WriteLine("multyply by number");
            matrixContext.MultiplyMatrixByNumber(5);
        }
    }
}
