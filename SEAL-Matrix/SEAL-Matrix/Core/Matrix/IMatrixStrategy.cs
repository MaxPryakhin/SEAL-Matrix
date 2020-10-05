using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public interface IMatrixStrategy
    {
        public double[] initMatrix(double[] matrix, int rowSize);
        public double[] multiplyMatrixByNumber(double[] matrix, double number);
    }
}
