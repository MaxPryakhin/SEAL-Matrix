using SEAL_Matrix.Core.Matrix;
using System;
using System.Data;
using Microsoft.Research.SEAL;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using SEAL_Matrix.Core.Enums;
using SEAL_Matrix.Core.Helpers;

namespace SEAL_Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            //int command = DisplayMenu();
            //test
            //int command = 3;



            var isExists = File.Exists("test.xlsx");
            using var fs = File.Open("test.xlsx", FileMode.OpenOrCreate);

            var package = new ExcelPackage(fs);
            if (!isExists)
            {
                package.Workbook.Worksheets.Add("0.Параметры криптосистемы");

                package.Workbook.Worksheets.Add("1. Объем ОЗУ для хранения матриц");
                package.Workbook.Worksheets.Add("1.1.Время шифрования матриц (сумма)");
                package.Workbook.Worksheets.Add("1.1.Объем ОЗУ для хранения зашифрованных  матриц (сумма)");
                package.Workbook.Worksheets.Add("1.1.Время дешифрования матриц (сумма)");
                package.Workbook.Worksheets.Add("1.2.Время шифрования матриц (умножение)");
                package.Workbook.Worksheets.Add("1.2.Объем ОЗУ для хранения зашифрованных  матриц (умножение)");
                package.Workbook.Worksheets.Add("1.2.Время дешифрования матриц (умножение)");

                package.Workbook.Worksheets.Add("2.Время сложения матриц");
                package.Workbook.Worksheets.Add("2.Объем ОЗУ для хранения матрицы сложения");
                package.Workbook.Worksheets.Add("2.Время сложения зашифрованных матриц");
                package.Workbook.Worksheets.Add("2.Объем ОЗУ для хранения зашифрованной матрицы сложения");
                package.Workbook.Worksheets.Add("2.Ошибка результата");

                package.Workbook.Worksheets.Add("3.Время умножения матрицы на число");
                package.Workbook.Worksheets.Add("3.Объем ОЗУ для хранения матрицы, умноженной на число");
                package.Workbook.Worksheets.Add("3.Время умноженния зашифрованной матрицы на число");
                package.Workbook.Worksheets.Add("3.Объем ОЗУ для хранения зашифрованной матрицы, умноженной на число");
                package.Workbook.Worksheets.Add("3.Ошибка результата");

                package.Workbook.Worksheets.Add("4.Время умножения матриц");
                package.Workbook.Worksheets.Add("4.Объем ОЗУ для хранения матрицы умножения");
                package.Workbook.Worksheets.Add("4.Время умножения зашифрованных матриц");
                package.Workbook.Worksheets.Add("4.Объем ОЗУ для хранения зашифрованной матрицы умножения");
                package.Workbook.Worksheets.Add("4.Ошибка результата");

                foreach (var workSheet in package.Workbook.Worksheets)
                {
                    workSheet.Cells[1, 1].Value = "c_id";
                    workSheet.Cells[1, 2].Value = "100x100";
                    workSheet.Cells[1, 3].Value = "200x200";
                    workSheet.Cells[1, 4].Value = "300x200";
                    workSheet.Cells[1, 5].Value = "400x400";
                    workSheet.Cells[1, 6].Value = "500x500";
                    workSheet.Cells[1, 7].Value = "600x600";
                    workSheet.Cells[1, 8].Value = "700x700";
                    workSheet.Cells[1, 9].Value = "800x800";
                    workSheet.Cells[1, 10].Value = "900x900";
                }
            }

            //package.Save();


            var startRow = 1;
            var cells = package.Workbook.Worksheets[0].Cells;
            var cell = cells[1, 1];
            int row = 1;

            do
            {
                row += 1;
                cell = cells[row, 1];
            } while (cell.Value != null);



            foreach (var workSheet in package.Workbook.Worksheets)
            {
                workSheet.Cells[row, 1].Value = row - 1;
            }

            var matrixStrategy = new MatrixStrategy();
            var matrixContext = new MatrixContext(matrixStrategy);
            var matrixHomomorphicStrategy = new MatrixHomomorphicStrategy(package, row);
            var matrixHomomorphicContext = new MatrixContext(matrixHomomorphicStrategy);

            var random = new Random();

            for (var i = 1; i < 10; i++)
            {
                Console.WriteLine($"{i}00x{i}00...");
                var size = 100 * i;
                var column = 1 + i;
                var bytesBefore = GC.GetTotalMemory(false);
                var firstMatrix = MatrixHelper.GenerateRandomMatrix(size);
                var bytesAfter = GC.GetTotalMemory(false);
                var secondMatrix = MatrixHelper.GenerateRandomMatrix(size);

                var sheet = package.Workbook.Worksheets[(int)TableEnum.Ram];
                var bytes = bytesAfter - bytesBefore;
                sheet.Cells[row, column].Value = bytes;


                var clearResult = matrixContext.AddMatrix(firstMatrix, secondMatrix, package, row, column);
                var decryptedResult = matrixHomomorphicContext.AddMatrix(firstMatrix, secondMatrix, package, row, column);
                var maxError = FindMaxAbsoluteError(clearResult, decryptedResult);
                sheet = package.Workbook.Worksheets[(int)TableEnum.SumResultError];
                sheet.Cells[row, column].Value = maxError;

                var number = random.NextDouble();
                clearResult = matrixContext.MultiplyMatrixByNumber(firstMatrix, number, package, row, column);
                decryptedResult = matrixHomomorphicContext.MultiplyMatrixByNumber(firstMatrix, number, package, row, column);
                maxError = FindMaxAbsoluteError(clearResult, decryptedResult);
                sheet = package.Workbook.Worksheets[(int)TableEnum.MulByNumberResultError];
                sheet.Cells[row, column].Value = maxError;

                //clearResult = matrixContext.MultiplyMatrix(firstMatrix, secondMatrix, package, row, column);
                //decryptedResult = matrixHomomorphicContext.MultiplyMatrix(firstMatrix, secondMatrix, package, row, column);
                //maxError = FindMaxAbsoluteError(clearResult, decryptedResult);
                //sheet = package.Workbook.Worksheets[(int)TableEnum.SumResultError];
                //sheet.Cells[row, column].Value = maxError;


            }

            package.SaveAs(fs);


            //while (command != 5)
            //{
            //    var firstMatrix = InitMatrix();
            //    //test
            //    //var firstMatrix = new Matrix()
            //    //{
            //    //    RowSize = 4,
            //    //    Elements = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}
            //    //};

            //    var firstCopyMatrix = new Matrix()
            //    {
            //        Elements = (double[])firstMatrix.Elements.Clone(),
            //        RowSize = firstMatrix.RowSize
            //    };

            //    matrixContext.Matrix = firstMatrix;
            //    matrixHomomorphicContext.Matrix = firstCopyMatrix;
            //    switch (command)
            //    {
            //        case 1:
            //            var number = InitNumber();
            //            matrixContext.MultiplyMatrixByNumber(number);
            //            matrixHomomorphicContext.MultiplyMatrixByNumber(number);
            //            break;
            //        case 2:
            //            var secondAddMatrix = InitMatrix();
            //            matrixContext.AddMatrix(secondAddMatrix);
            //            matrixHomomorphicContext.AddMatrix(secondAddMatrix);
            //            break;
            //        case 3:
            //            var secondMultMatrix = InitMatrix();
            //            //test
            //            //var secondMultMatrix = new Matrix()
            //            //{
            //            //    Elements = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 },
            //            //    RowSize = firstMatrix.RowSize
            //            //};
            //            matrixContext.MultiplyMatrix(secondMultMatrix);
            //            matrixHomomorphicContext.MultiplyMatrix(secondMultMatrix);
            //            break;
            //    }

            //    if (command != 5)
            //    {
            //        CompareMatrix(matrixContext.Matrix, matrixHomomorphicContext.Matrix);
            //    }

            //    command = DisplayMenu();
            //}
        }

        private static double FindMaxAbsoluteError(Matrix clearResult, Matrix decryptedResult)
        {
            double maxError = 0;
            for (int i = 0; i < clearResult.Elements.LongLength; i++)
            {
                var clear = clearResult.Elements[i];
                var decrypted = decryptedResult.Elements[i];

                var error = Math.Abs(decrypted - clear);

                if (error > maxError)
                {
                    maxError = error;
                }
            }

            return maxError;
        }

        static void InitTable(ExcelWorksheet worksheet)
        {
            worksheet.Cells[0, 0].Value = "id_c";

        }

        static public int DisplayMenu()
        {
            Console.WriteLine("Matrix operation");
            Console.WriteLine();
            Console.WriteLine("1. Multiply matrix by number");
            Console.WriteLine("2. Add matrix to matrix");
            Console.WriteLine("3. Multiply matrix");
            //Console.WriteLine("4. ");
            Console.WriteLine("5. Exit");
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }

        static public Matrix InitMatrix()
        {
            Console.WriteLine("Row size (2): ");
            var rowSize = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Matrix (1,0 2,0 0,5 5,0 ...): ");
            var result = Console.ReadLine().ToString(CultureInfo.InvariantCulture);
            var elements = result.Split(" ").Select(s => Convert.ToDouble(s)).ToArray();
            return new Matrix
            {
                Elements = elements,
                RowSize = rowSize
            };
        }

        static public double InitNumber()
        {
            Console.WriteLine("Number (5,0): ");
            return Convert.ToDouble(Console.ReadLine());
        }

        static public void CompareMatrix(Matrix a, Matrix b)
        {
            Console.WriteLine();
            Console.WriteLine("Compare matrix: ");
            Console.WriteLine();
            var aElements = a.Elements;
            var bElements = b.Elements;
            for (int i = 0; i < aElements.Length; i++)
            {
                Console.WriteLine($"element at {i}");
                Console.WriteLine($"{aElements[i]}");
                Console.WriteLine($"{bElements[i]}");
                Console.WriteLine();
            }
            Console.WriteLine($"/{new string('-', 10)}/");
            Console.WriteLine();
        }
    }
}
