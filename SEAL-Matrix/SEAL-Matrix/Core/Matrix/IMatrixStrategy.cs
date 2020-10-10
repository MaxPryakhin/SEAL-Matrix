using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public interface IMatrixStrategy
    {
        public Matrix MultiplyMatrixByNumber(Matrix matrix, double number);

        public Matrix SumMatrix (Matrix a, Matrix b);

        public Matrix MultiplyMatrix(Matrix a, Matrix b);
    }
}
