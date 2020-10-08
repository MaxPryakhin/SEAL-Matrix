using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public interface IMatrixStrategy
    {
        public double[] MultiplyMatrixByNumber(Matrix matrix, double number);

        public double[] SumMatrix (Matrix a, Matrix b);
    }
}
