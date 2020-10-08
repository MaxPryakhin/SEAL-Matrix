using SEAL_Matrix.Core.Matrix;
using System;
using Microsoft.Research.SEAL;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace SEAL_Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            int command = DisplayMenu();

            var matrixStrategy = new MatrixStrategy();
            var matrixContext = new MatrixContext(matrixStrategy);
            var matrixHomomorphicStrategy = new MatrixHomomorphicStrategy();
            var matrixHomomorphicContext = new MatrixContext(matrixHomomorphicStrategy);

            while (command != 5)
            {
                var firstMatrix = InitMatrix();
                var firstCopyMatrix = new Matrix()
                {
                    Elements = (double[])firstMatrix.Elements.Clone(),
                    RowSize = firstMatrix.RowSize
                };

                matrixContext.Matrix = firstMatrix;
                matrixHomomorphicContext.Matrix = firstCopyMatrix;
                switch (command)
                {
                    case 1:
                        var number = InitNumber();
                        matrixContext.MultiplyMatrixByNumber(number);
                        matrixHomomorphicContext.MultiplyMatrixByNumber(number);
                        break;
                    case 2:
                        var secondAddMatrix = InitMatrix();
                        matrixContext.AddMatrix(secondAddMatrix);
                        matrixHomomorphicContext.AddMatrix(secondAddMatrix);
                        break;
                }

                if (command != 5)
                {
                    CompareMatrix(firstMatrix, firstCopyMatrix);
                }

                command = DisplayMenu();
            }
        }

        static public int DisplayMenu()
        {
            Console.WriteLine("Matrix operation");
            Console.WriteLine();
            Console.WriteLine("1. Multiply matrix by number");
            Console.WriteLine("2. Add matrix to matrix");
            //Console.WriteLine("3. ");
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
