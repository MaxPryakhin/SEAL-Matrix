using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixContext
    {
        private IMatrixStrategy _strategy;

        public MatrixContext(IMatrixStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(IMatrixStrategy));
        }

        public Matrix MultiplyMatrixByNumber(Matrix matrix,double number, ExcelPackage package, int row, int column)
        {
            return _strategy.MultiplyMatrixByNumber(matrix, number, package, row, column);
        }

        public Matrix AddMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column)
        {
            return _strategy.SumMatrix(a, b, package, row, column);
        }

        public Matrix MultiplyMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column)
        {
            return _strategy.MultiplyMatrix(a, b, package, row, column);
        }
    }
}
