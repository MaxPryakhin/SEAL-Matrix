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
            
            Console.WriteLine("initial matrix");
            //TODO: change init
            matrixContext.Matrix = matrix;

            //Console.WriteLine("multiply by number");
            //matrixContext.MultiplyMatrixByNumber(5);

            Console.WriteLine("add matrix");
            matrixContext.AddMatrix(matrix);

            var matrixHomomorphicStrategy = new MatrixHomomorphicStrategy();
            var matrixHomomorphicContext = new MatrixContext(matrixHomomorphicStrategy);

            Console.WriteLine("initial homomorphic matrix");
            matrix = new Matrix
            {
                Elements = new double[] { 1.0, 0.5, 2.0, 5.0 },
                RowSize = 2
            };
            //TODO: change init
            matrixHomomorphicContext.Matrix = matrix;

            //Console.WriteLine("multiply by number");
            //matrixHomomorphicContext.MultiplyMatrixByNumber(5);

            Console.WriteLine("add matrix");
            matrixHomomorphicContext.AddMatrix(matrix);
        }
    }
}
