using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixStrategy : IMatrixStrategy
    {
        public double[] multiplyMatrixByNumber(double[] matrix, double number)
        {
            return matrix.Select(m => m * number).ToArray();
        }
    }
}
