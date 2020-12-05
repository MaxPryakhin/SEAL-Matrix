using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;

namespace SEAL_Matrix.Core.Matrix
{
    public interface IMatrixStrategy
    {
        public Matrix MultiplyMatrixByNumber(Matrix matrix, double number, ExcelPackage package, int row, int column);

        public Matrix SumMatrix (Matrix a, Matrix b, ExcelPackage package, int row, int column);

        public Matrix MultiplyMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column);
    }
}
