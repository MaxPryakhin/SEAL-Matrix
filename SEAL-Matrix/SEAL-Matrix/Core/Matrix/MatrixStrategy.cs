using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using SEAL_Matrix.Core.Enums;
using SEAL_Matrix.Core.Helpers;

namespace SEAL_Matrix.Core.Matrix
{
    public class MatrixStrategy : IMatrixStrategy
    {
        public Matrix MultiplyMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var aRowSize = a.RowSize;
            var aColumnSize = a.Elements.Length / aRowSize;
            var aElements = a.Elements;
            var bRowSize = b.RowSize;
            var bColumnSize = b.Elements.Length / bRowSize;
            var bElements = b.Elements;

            if(aColumnSize != bRowSize)
            {
                throw new ArgumentException(
                    "row size of first matrix is not equal with column size of second matrix"
                    );
            }

            var bytesBefore = GC.GetTotalMemory(false);
            var elements = new double[aColumnSize * bRowSize];
            for (var i = 0; i < aColumnSize; i++)
            {
                for (var j = 0; j < bRowSize; j++)
                {
                    for (var k = 0; k < aRowSize; k++)
                    {
                        elements[i * bRowSize + j] = 
                            elements[i * bRowSize + j] 
                            + aElements[i * aRowSize + k] * bElements[k * bRowSize + j];
                    }
                }
            }
            
            var resultMatrix = new Matrix()
            {
                Elements = elements,
                RowSize = aRowSize
            };
            var bytesAfter = GC.GetTotalMemory(false);
            stopwatch.Stop();

            var sheet = package.Workbook.Worksheets[(int)TableEnum.MulTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            sheet = package.Workbook.Worksheets[(int)TableEnum.MulRam];
            var bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            return resultMatrix;
        }

        public Matrix MultiplyMatrixByNumber(Matrix matrix, double number, ExcelPackage package, int row, int column)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var bytesBefore = GC.GetTotalMemory(false);
            var resultMatrix = new Matrix()
            {
                Elements = matrix.Elements.Select(e => e * number).ToArray(),
                RowSize = matrix.RowSize
            };
            var bytesAfter = GC.GetTotalMemory(false);

            stopwatch.Stop();

            var sheet = package.Workbook.Worksheets[(int)TableEnum.MulByNumberTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            sheet = package.Workbook.Worksheets[(int)TableEnum.MulByNumberRam];
            var bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            return resultMatrix;
        }

        public Matrix SumMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column)
        {
            var length = a.Elements.Length;
            if (length != b.Elements.Length)
            {
                throw new ArgumentException("Length is not equal");
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var bytesBefore = GC.GetTotalMemory(false);
            var result = new double[length];
            for (var i = 0; i < a.Elements.Length; i++)
            {
                result[i] = a.Elements[i] + b.Elements[i];
            }

            var resultMatrix = new Matrix()
            {
                Elements = result,
                RowSize = a.RowSize
            };
            var bytesAfter = GC.GetTotalMemory(false);

            stopwatch.Stop();

            var sheet = package.Workbook.Worksheets[(int)TableEnum.SumTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            sheet = package.Workbook.Worksheets[(int)TableEnum.SumRam];
            var bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            return resultMatrix;
        }     
    }
}
