using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixStrategy : IMatrixStrategy
    {
        public Matrix MultiplyMatrix(Matrix a, Matrix b)
        {
            var aRowSize = a.RowSize;
            var aColumnSize = a.Elements.Length / aRowSize;
            var aElements = a.Elements;
            var bRowSize = b.RowSize;
            var bColumnSize = b.Elements.Length / bRowSize;
            var bElements = b.Elements;

            if(aColumnSize != bRowSize)
            {
                throw new ArgumentException(
                    "row size of first matrix is not equal with column size of second matrix"
                    );
            }

            var elements = new double[aColumnSize * bRowSize];
            for (int i = 0; i < aColumnSize; i++)
            {
                for (int j = 0; j < bRowSize; j++)
                {
                    for (int k = 0; k < aRowSize; k++)
                    {
                        elements[i * bRowSize + j] = 
                            elements[i * bRowSize + j] 
                            + aElements[i * aRowSize + k] * bElements[k * bRowSize + j];
                    }
                }
            }

            return new Matrix()
            {
                Elements = elements,
                RowSize = aRowSize
            };
        }

        public Matrix MultiplyMatrixByNumber(Matrix matrix, double number)
        {
            return new Matrix()
            {
                Elements = matrix.Elements.Select(e => e * number).ToArray(),
                RowSize = matrix.RowSize
            };   
        }

        public Matrix SumMatrix(Matrix a, Matrix b)
        {
            var length = a.Elements.Length;
            if (length != b.Elements.Length)
            {
                throw new ArgumentException("Length is not equal");
            }

            var result = new double[length];
            for (int i = 0; i < a.Elements.Length; i++)
            {
                result[i] = a.Elements[i] + b.Elements[i];
            }

            return new Matrix()
            {
                Elements = result,
                RowSize = a.RowSize
            };
        }     
    }
}
