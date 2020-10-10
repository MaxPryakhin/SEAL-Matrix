using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixContext
    {
        private IMatrixStrategy _strategy;
        public Matrix Matrix { get; set; }

        public MatrixContext(IMatrixStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(IMatrixStrategy));
        }

        public void MultiplyMatrixByNumber(double number)
        {
            Matrix = _strategy.MultiplyMatrixByNumber(Matrix, number);
        }

        public void AddMatrix(Matrix matrix)
        {
            Matrix = _strategy.SumMatrix(Matrix, matrix);
        }

        public void MultiplyMatrix(Matrix matrix)
        {
            Matrix = _strategy.MultiplyMatrix(Matrix, matrix);
        }
    }
}
