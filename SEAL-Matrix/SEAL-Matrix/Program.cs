using SEAL_Matrix.Core.Matrix;
using System;
using Microsoft.Research.SEAL;

namespace SEAL_Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = new Matrix
            {
                Elements = new double[] { 1.0, 0.5, 2.0, 5.0 },
                RowSize = 2
            };

            var matrixStrategy = new MatrixStrategy();
            var matrixContext = new MatrixContext(matrixStrategy);
            
            Console.WriteLine("init matrix");
            matrixContext.Matrix = matrix;

            Console.WriteLine("multyply by number");
            matrixContext.MultiplyMatrixByNumber(5);

            var matrixHomomorphicStrategy = new MatrixHomomorphicStrategy();
            var matrixHomomorphicContext = new MatrixContext(matrixHomomorphicStrategy);

            Console.WriteLine("init homomorphic matrix");
            matrixHomomorphicContext.Matrix = matrix;

            Console.WriteLine("multyply by number");
            matrixHomomorphicContext.MultiplyMatrixByNumber(5);

        }
    }
}
