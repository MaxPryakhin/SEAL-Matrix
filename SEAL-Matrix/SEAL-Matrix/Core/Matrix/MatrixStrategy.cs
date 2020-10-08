using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixStrategy : IMatrixStrategy
    {
        public double[] MultiplyMatrixByNumber(Matrix matrix, double number)
        {
            return matrix.Elements.Select(e => e * number).ToArray();
        }

        public double[] SumMatrix(Matrix a, Matrix b)
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

            return result;
        }
    }
}
