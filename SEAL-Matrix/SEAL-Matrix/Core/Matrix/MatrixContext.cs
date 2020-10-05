using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixContext
    {
        private IMatrixStrategy _strategy;
        public double[] Matrix { private set; get; }
        public int RowSize { private set; get; }

        public MatrixContext(IMatrixStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(IMatrixStrategy));
        }

        public void InitMatrix(double[] matrix, int rowSize)
        {
            Matrix = _strategy.initMatrix(matrix, rowSize);
            RowSize = rowSize;
        }

        public void MultiplyMatrixByNumber(double number)
        {
            Matrix = _strategy.multiplyMatrixByNumber(Matrix, number);
        }

    }
}
