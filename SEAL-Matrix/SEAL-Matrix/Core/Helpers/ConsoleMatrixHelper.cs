using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Helpers
{
    public static class ConsoleMatrixHelper
    {
        public static void PrintMatrix(Matrix.Matrix matrix, int? maxSize = null)
        {
            var elements = matrix.Elements;
            var rowSize = matrix.RowSize;
            var colSize = maxSize ?? elements.Length / rowSize;

            for (var i = 0; i < rowSize; i++)
            {
                Console.Write("[");
                for (int j = 0; j < colSize; j++)
                {
                    var element = elements[i * rowSize + j];
                    Console.Write(element);

                    if(j != colSize - 1)
                    {
                        Console.Write(", ");
                    }

                }
                Console.WriteLine("]");
            }
        }

        public static void PrintMatrix(double[,] matrix, int? maxSize = null)
        {
            var rowSize = matrix.GetLength(0);
            var colSize = maxSize ??  matrix.GetLength(1);
            for (var i = 0; i < rowSize; i++)
            {
                Console.Write("[");
                for (var j = 0; j < colSize; j++)
                {
                    var element = matrix[i, j];
                    Console.Write(element);

                    if (j != colSize - 1)
                    {
                        Console.Write(", ");
                    }

                }
                Console.WriteLine("]");
            }
        }

        public static void PrintMatrix(List<double[]> matrix, int? maxSize = null)
        {
            var rowSize = matrix.Count;
            var colSize = maxSize ?? matrix[0].Length;
            for (var i = 0; i < rowSize; i++)
            {
                Console.Write("[");
                for (var j = 0; j < colSize; j++)
                {
                    var element = matrix[i][j];
                    Console.Write(element);

                    if (j != colSize - 1)
                    {
                        Console.Write(", ");
                    }

                }
                Console.WriteLine("]");
            }
        }
    }
}
