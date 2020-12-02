using SEAL_Matrix.Core.Matrix;
using System;
using Microsoft.Research.SEAL;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using SEAL_Matrix.Core.Helpers;

namespace SEAL_Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            //int command = DisplayMenu();
            //test
            //int command = 3;

            var matrixStrategy = new MatrixStrategy();
            var matrixContext = new MatrixContext(matrixStrategy);
            var matrixHomomorphicStrategy = new MatrixHomomorphicStrategy();
            var matrixHomomorphicContext = new MatrixContext(matrixHomomorphicStrategy);

            //while (command != 5)
            //{
            //    var firstMatrix = InitMatrix();
            //    //test
            //    //var firstMatrix = new Matrix()
            //    //{
            //    //    RowSize = 4,
            //    //    Elements = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}
            //    //};

            //    var firstCopyMatrix = new Matrix()
            //    {
            //        Elements = (double[])firstMatrix.Elements.Clone(),
            //        RowSize = firstMatrix.RowSize
            //    };

            //    matrixContext.Matrix = firstMatrix;
            //    matrixHomomorphicContext.Matrix = firstCopyMatrix;
            //    switch (command)
            //    {
            //        case 1:
            //            var number = InitNumber();
            //            matrixContext.MultiplyMatrixByNumber(number);
            //            matrixHomomorphicContext.MultiplyMatrixByNumber(number);
            //            break;
            //        case 2:
            //            var secondAddMatrix = InitMatrix();
            //            matrixContext.AddMatrix(secondAddMatrix);
            //            matrixHomomorphicContext.AddMatrix(secondAddMatrix);
            //            break;
            //        case 3:
            //            var secondMultMatrix = InitMatrix();
            //            //test
            //            //var secondMultMatrix = new Matrix()
            //            //{
            //            //    Elements = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 },
            //            //    RowSize = firstMatrix.RowSize
            //            //};
            //            matrixContext.MultiplyMatrix(secondMultMatrix);
            //            matrixHomomorphicContext.MultiplyMatrix(secondMultMatrix);
            //            break;
            //    }

            //    if (command != 5)
            //    {
            //        CompareMatrix(matrixContext.Matrix, matrixHomomorphicContext.Matrix);
            //    }

            //    command = DisplayMenu();
            //}
        }

        static public int DisplayMenu()
        {
            Console.WriteLine("Matrix operation");
            Console.WriteLine();
            Console.WriteLine("1. Multiply matrix by number");
            Console.WriteLine("2. Add matrix to matrix");
            Console.WriteLine("3. Multiply matrix");
            //Console.WriteLine("4. ");
            Console.WriteLine("5. Exit");
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }

        static public Matrix InitMatrix()
        {
            Console.WriteLine("Row size (2): ");
            var rowSize = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Matrix (1,0 2,0 0,5 5,0 ...): ");
            var result = Console.ReadLine().ToString(CultureInfo.InvariantCulture);
            var elements = result.Split(" ").Select(s => Convert.ToDouble(s)).ToArray();
            return new Matrix
            {
                Elements = elements,
                RowSize = rowSize
            };
        }

        static public double InitNumber()
        {
            Console.WriteLine("Number (5,0): ");
            return Convert.ToDouble(Console.ReadLine());
        }

        static public void CompareMatrix(Matrix a, Matrix b)
        {
            Console.WriteLine();
            Console.WriteLine("Compare matrix: ");
            Console.WriteLine();
            var aElements = a.Elements;
            var bElements = b.Elements;
            for (int i = 0; i < aElements.Length; i++)
            {
                Console.WriteLine($"element at {i}");
                Console.WriteLine($"{aElements[i]}");
                Console.WriteLine($"{bElements[i]}");
                Console.WriteLine();
            }
            Console.WriteLine($"/{new string('-', 10)}/");
            Console.WriteLine();
        }
    }
}
