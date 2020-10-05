using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixStrategy : IMatrixStrategy
    {
        public double[] initMatrix(double[] matrix, int rowSize)
        {
            return matrix;
        }

        public double[] multiplyMatrixByNumber(double[] matrix, double number)
        {
            return matrix.Select(m => m * number).ToArray();
        }
    }
}
