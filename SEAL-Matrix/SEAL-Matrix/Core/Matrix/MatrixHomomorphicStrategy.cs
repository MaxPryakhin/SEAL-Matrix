using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Research.SEAL;
using OfficeOpenXml;
using SEAL_Matrix.Core.Enums;
using SEAL_Matrix.Core.Helpers;

namespace SEAL_Matrix.Core.Matrix
{
    class MatrixHomomorphicStrategy : IMatrixStrategy
    {
        private EncryptionParameters _parms;

        public MatrixHomomorphicStrategy()
        {
            var parms = new EncryptionParameters(SchemeType.CKKS);

            const ulong polyModulusDegree = 16384;
            //Console.WriteLine($"Max bit count ${CoeffModulus.MaxBitCount(polyModulusDegree)}");
            parms.PolyModulusDegree = polyModulusDegree;
            parms.CoeffModulus = CoeffModulus.Create(
                polyModulusDegree, new int[] { 60, 40, 40, 40, 40, 60 });

            _parms = parms;
        }

        public Matrix MultiplyMatrix(Matrix a, Matrix b, ExcelPackage package, int row, int column)
        {
            using SEALContext context = new SEALContext(_parms);
            using CKKSEncoder ckksEncoder = new CKKSEncoder(context);

            ulong slotCount = ckksEncoder.SlotCount;

            var length = a.RowSize;
            var square = length * length;
            if (a.Elements.Length != b.Elements.Length)
            {
                throw new ArgumentException("Length is not equal");
            }

            //Console.WriteLine("Matrix 1");
            //ConsoleMatrixHelper.PrintMatrix(a);
            //Console.WriteLine("Matrix 2");
            //ConsoleMatrixHelper.PrintMatrix(b);


            using var keygen = new KeyGenerator(context);
            using var publicKey = keygen.PublicKey;
            using var secretKey = keygen.SecretKey;
            using var galKeys = keygen.GaloisKeysLocal();
            using var encryptor = new Encryptor(context, publicKey);
            using var evaluator = new Evaluator(context);
            using var decryptor = new Decryptor(context, secretKey);

            var rowCount1 = a.Elements.Length / a.RowSize;
            var rowCount2 = b.Elements.Length / b.RowSize;
            var podMatrix1 = new double[rowCount1, rowCount1];
            var podMatrix2 = new double[rowCount2, rowCount2];
            var elems1 = a.Elements;
            var elems2 = b.Elements;
            Plaintext[] plainMatrix1 = new Plaintext[rowCount1],
                            plainMatrix2 = new Plaintext[rowCount2];
            Ciphertext[] encryptedMatrix1 = new Ciphertext[rowCount1],
                             encryptedMatrix2 = new Ciphertext[rowCount2];
            //Ciphertext encodeMatrix1, encodeMatrix2;
            var scale = Math.Pow(2.0, 40);

            var rowSize = a.RowSize;
            var colSize = a.Elements.Length / rowSize;
            var aList = new List<double[]>();
            var bList = new List<double[]>();
            for (var i = 0; i < rowSize; i++)
            {
                var aRow = new double[colSize];
                var bRow = new double[colSize];
                for (int j = 0; j < colSize; j++)
                {
                    aRow[j] = a.Elements[i * rowSize + j];
                    bRow[j] = b.Elements[i * rowSize + j];
                }
                aList.Add(aRow);
                bList.Add(bRow);
            }


            for (var r = 0; r < rowCount1; r++)
            {
                var array = new double[rowCount1];
                for (var i = 0; i < a.RowSize; i++)
                {
                    podMatrix1[r, i] = aList[r][i];
                    
                }
            }

            var uSigma = MatrixHelper.GetUSigma(podMatrix1);
            //Console.WriteLine("u sigma");
            //ConsoleMatrixHelper.PrintMatrix(uSigma);

            for (var r = 0; r < rowCount2; r++)
            {
                var array = new double[rowCount1];
                for (var i = 0; i < a.RowSize; i++)
                {
                    podMatrix2[r, i] = bList[r][i];

                }
            }

            var uTau = MatrixHelper.GetUTau(podMatrix2);
            //Console.WriteLine("u tau");
            //ConsoleMatrixHelper.PrintMatrix(uTau);

            var vK = new List<double[,]>(length - 1);

            for (var i = 1; i < length; i++)
            {
                vK.Add(MatrixHelper.GetVk(podMatrix1, i));
            }

            //for (var i = 0; i < vK.Count; i++)
            //{
            //    Console.WriteLine($"V {i + 1}");
            //    ConsoleMatrixHelper.PrintMatrix(vK[i]);
            //}

            var wK = new List<double[,]>(length - 1);

            for (var i = 1; i < length; i++)
            {
                wK.Add(MatrixHelper.GetWk(podMatrix1, i));
            }

            //for (var i = 0; i < wK.Count; i++)
            //{
            //    Console.WriteLine($"W {i + 1}");
            //    ConsoleMatrixHelper.PrintMatrix(wK[i]);
            //}

            var uSigmaDiagonals = MatrixHelper.GetAllDiagonals(uSigma);
            var uTauDiagonals = MatrixHelper.GetAllDiagonals(uTau);
            var epsilon = Math.Pow(10, -8);


            //Console.WriteLine("uSigmaDiagonals");
            //ConsoleMatrixHelper.PrintMatrix(uSigmaDiagonals);

            //Console.WriteLine("uTauDiagonals");
            //ConsoleMatrixHelper.PrintMatrix(uTauDiagonals);

            for (var i = 0; i < square; i++)
            {
                for (var j = 0; j < square; j++)
                {
                    uSigmaDiagonals[i][j] += epsilon;
                    uTauDiagonals[i][j] += epsilon;
                }
            }

            var vkDiagonals = new List<List<double[]>>(length - 1);
            var wkDiagonals = new List<List<double[]>>(length - 1);
            for (var i = 1; i < length; i++)
            {
                vkDiagonals.Add(MatrixHelper.GetAllDiagonals(vK[i - 1]));
                wkDiagonals.Add(MatrixHelper.GetAllDiagonals(wK[i - 1]));
            }

            // Test ADD EPSILON
            for (var i = 0; i < length - 1; i++)
            {
                for (var j = 0; j < square; j++)
                {
                    for (var k = 0; k < square; k++)
                    {

                        vkDiagonals[i][j][k] += epsilon;
                        wkDiagonals[i][j][k] += epsilon;
                    }
                }
            }

            var stopwatch = new Stopwatch();

            var uSigmaDiagonalsPlain = new Plaintext[square];
            var uTauDiagonalsPlain = new Plaintext[square];
            for (var i = 0; i < square; i++)
            {
                uSigmaDiagonalsPlain[i] = new Plaintext();
                ckksEncoder.Encode(uSigmaDiagonals[i], scale, uSigmaDiagonalsPlain[i]);
                uTauDiagonalsPlain[i] = new Plaintext();
                ckksEncoder.Encode(uTauDiagonals[i], scale, uTauDiagonalsPlain[i]);
            }

            var vkdIagonalsPlain = new List<Plaintext[]>(length - 1);
            var wkdIagonalsPlain = new List<Plaintext[]>(length - 1);

            for (int i = 1; i < length; i++)
            {
                vkdIagonalsPlain.Add(new Plaintext[square]);
                wkdIagonalsPlain.Add(new Plaintext[square]);
                for (int j = 0; j < square; j++)
                {
                    vkdIagonalsPlain[i - 1][j] = new Plaintext();
                    ckksEncoder.Encode(vkDiagonals[i - 1][j], scale, vkdIagonalsPlain[i - 1][j]);
                    wkdIagonalsPlain[i - 1][j] = new Plaintext();
                    ckksEncoder.Encode(wkDiagonals[i - 1][j], scale, wkdIagonalsPlain[i - 1][j]);
                }
            }

            var ctAResult = new Ciphertext[length];
            var ctBResult = new Ciphertext[length];

            for (int r = 0; r < length; r++)
            {
                var array1 = new double[rowCount1];
                var array2 = new double[rowCount2];

                for (int i = 0; i < a.RowSize; i++)
                {
                    array1[i] = aList[r][i];
                }


                for (int i = 0; i < a.RowSize; i++)
                {
                    array2[i] = bList[r][i];
                }

                plainMatrix1[r] = new Plaintext();
                ckksEncoder.Encode(array1, scale, plainMatrix1[r]);
                encryptedMatrix1[r] = new Ciphertext();
                encryptor.Encrypt(plainMatrix1[r], encryptedMatrix1[r]);
                plainMatrix2[r] = new Plaintext();
                ckksEncoder.Encode(array2, scale, plainMatrix2[r]);
                encryptedMatrix2[r] = new Ciphertext();
                encryptor.Encrypt(plainMatrix2[r], encryptedMatrix2[r]);
            }

            var bytesBefore = GC.GetTotalMemory(false);
            using Ciphertext encodeMatrix1 = EncodeMatrix(encryptedMatrix1, galKeys, evaluator);
            var bytesAfter = GC.GetTotalMemory(false);
            using Ciphertext encodeMatrix2 = EncodeMatrix(encryptedMatrix2, galKeys, evaluator);

            stopwatch.Stop();

            var sheet = package.Workbook.Worksheets[(int)TableEnum.MulEncryptingTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            sheet = package.Workbook.Worksheets[(int)TableEnum.MulEncryptingRam];
            var bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            stopwatch.Reset();
            stopwatch.Start();

            foreach (var ciphertext in encryptedMatrix1)
            {
                var testPlain = new Plaintext();
                decryptor.Decrypt(ciphertext, testPlain);
                ckksEncoder.Decode(testPlain, new List<double>());
            }

            stopwatch.Stop();
            sheet = package.Workbook.Worksheets[(int)TableEnum.MulDecryptingTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();

            // Step 1-1
            ctAResult[0] = LinearTransformPlain(encodeMatrix1, uSigmaDiagonalsPlain, galKeys);

            // Step 1-2
            ctBResult[0] = LinearTransformPlain(encodeMatrix2, uTauDiagonalsPlain, galKeys);

            // Step 2
            for (var k = 1; k < length; k++)
            {
                //Console.WriteLine($"step 2 k = {k}");
                //Console.WriteLine("matrix A[0]");
                //TestDecryptMatrix(ctAResult[0], length, decryptor, ckksEncoder);
                //Console.WriteLine("matrix B[0]");
                //TestDecryptMatrix(ctBResult[0], length, decryptor, ckksEncoder);
                //Console.WriteLine("matrix VK");
                //ConsoleMatrixHelper.PrintMatrix(vkDiagonals[k - 1], length);
                //Console.WriteLine("matrix WK");
                //ConsoleMatrixHelper.PrintMatrix(wkDiagonals[k - 1], length);

                ctAResult[k] = LinearTransformPlain(ctAResult[0], vkdIagonalsPlain[k - 1], galKeys);
                ctBResult[k] = LinearTransformPlain(ctBResult[0], wkdIagonalsPlain[k - 1], galKeys);
            }

            //Console.WriteLine("step 1 and step 2 Linear");
            //Console.WriteLine("matrix A");
            //TestDecryptMatrix(ctAResult, length, decryptor, ckksEncoder);
            //Console.WriteLine("matrix B");
            //TestDecryptMatrix(ctBResult, length, decryptor, ckksEncoder);


            // Step 3
            for (var i = 1; i < length; i++)
            {
                evaluator.RescaleToNextInplace(ctAResult[i]);
                evaluator.RescaleToNextInplace(ctBResult[i]);
            }

            //Console.WriteLine("rescale result");
            //Console.WriteLine("matrix A res");
            //TestDecryptMatrix(ctAResult, length, decryptor, ckksEncoder);
            //Console.WriteLine("matrix B res");
            //TestDecryptMatrix(ctBResult, length, decryptor, ckksEncoder);
            bytesBefore = GC.GetTotalMemory(false);
            var ctAB = new Ciphertext();
            evaluator.Multiply(ctAResult[0], ctBResult[0], ctAB);

            //Console.WriteLine();
            //Console.WriteLine("rescale result");
            //Console.WriteLine("matrix AB[0]");
            //TestDecryptMatrix(ctAB, square, decryptor, ckksEncoder);

            evaluator.ModSwitchToNextInplace(ctAB);

            //Console.WriteLine();
            //Console.WriteLine("mod switch result");
            //Console.WriteLine("matrix A res");
            //TestDecryptMatrix(ctAResult, length, decryptor, ckksEncoder);
            //Console.WriteLine("matrix B res");
            //TestDecryptMatrix(ctBResult, length, decryptor, ckksEncoder);

            for (var i = 1; i < length; i++)
            {
                ctAResult[i].Scale = Math.Pow(2, (int)Math.Log2(ctAResult[i].Scale));
                ctBResult[i].Scale = Math.Pow(2, (int)Math.Log2(ctBResult[i].Scale));
            }

            for (var k = 1; k < length; k++)
            {
                var tempMul = new Ciphertext(context);
                evaluator.Multiply(ctAResult[k], ctBResult[k], tempMul);

                //Console.WriteLine();
                //Console.WriteLine("result temp mul");
                //Console.WriteLine($"temp mul {k}");
                //TestDecryptMatrix(ctAB, square, decryptor, ckksEncoder);

                evaluator.AddInplace(ctAB, tempMul);

                //Console.WriteLine();
                //Console.WriteLine($"add inplace matrix AB[{k}]");
                //TestDecryptMatrix(ctAB, square, decryptor, ckksEncoder);
            }

            //Console.WriteLine("step 3 result");
            //Console.WriteLine("matrix A");
            //TestDecryptMatrix(ctAResult, length, decryptor, ckksEncoder);
            //Console.WriteLine("matrix B");
            //TestDecryptMatrix(ctBResult, length, decryptor, ckksEncoder);
            bytesAfter = GC.GetTotalMemory(false);
            using Plaintext plainResult = new Plaintext();
            decryptor.Decrypt(ctAB, plainResult);
            var result = new List<double>();
            ckksEncoder.Decode(plainResult, result);

            stopwatch.Stop();
            sheet = package.Workbook.Worksheets[(int) TableEnum.MulEncryptOperationTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            var resultMatrix = new Matrix()
            {
                Elements = result.ToArray(),
                RowSize = a.RowSize,
            };

            sheet = package.Workbook.Worksheets[(int) TableEnum.MulEncryptResultRam];
            bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            //Console.WriteLine("result");
            //ConsoleMatrixHelper.PrintMatrix(resultMatrix, rowSize);

            return resultMatrix;
        }

        private static void TestDecryptMatrix(Ciphertext[] ctA_result, int length, Decryptor decryptor, CKKSEncoder ckksEncoder)
        {
            var test = new List<double[]>();

            for (var i = 0; i < length; i++)
            {
                var ctaLinPlain = new Plaintext();
                decryptor.Decrypt(ctA_result[i], ctaLinPlain);
                var ctaLinResult = new List<double>();
                ckksEncoder.Decode(ctaLinPlain, ctaLinResult);

                test.Add(ctaLinResult.ToArray());
            }

            ConsoleMatrixHelper.PrintMatrix(test, length);
        }

        private static void TestDecryptMatrix(Ciphertext ctAResult, int length, Decryptor decryptor, CKKSEncoder ckksEncoder)
        {
            var test = new List<double[]>();

            var ctaLinPlain = new Plaintext();
            decryptor.Decrypt(ctAResult, ctaLinPlain);
            var ctaLinResult = new List<double>();
            ckksEncoder.Decode(ctaLinPlain, ctaLinResult);

            test.Add(ctaLinResult.ToArray());

            ConsoleMatrixHelper.PrintMatrix(test, length);
        }

        Ciphertext EncodeMatrix(Ciphertext[] matrix, GaloisKeys galKeys, Evaluator evaluator)
        {
            Ciphertext ctResult = new Ciphertext();
            int length = matrix.GetLength(0);
            var ctRots = new Ciphertext[length];
            ctRots[0] = matrix[0];

            for (int i = 1; i < length; i++)
            {
                ctRots[i] = new Ciphertext();
                evaluator.RotateVector(matrix[i], (i * -length), galKeys, ctRots[i]);
            }

            evaluator.AddMany(ctRots, ctResult);

            return ctResult;
        }

        public Ciphertext LinearTransformPlain(Ciphertext ct, Plaintext[] uDiagonals, GaloisKeys gal_keys)
        {
            using SEALContext context = new SEALContext(_parms);
            using Evaluator evaluator = new Evaluator(context);

            // Fill ct with duplicate
            var ctRot = new Ciphertext();
            evaluator.RotateVector(ct, -uDiagonals.GetLength(0), gal_keys, ctRot);
            var ctNew = new Ciphertext();
            evaluator.Add(ct, ctRot, ctNew);

            var ctResult = new List<Ciphertext>(uDiagonals.GetLength(0));
            ctResult.Add(new Ciphertext());
            evaluator.MultiplyPlain(ctNew, uDiagonals[0], ctResult[0]);

            var tempRot = new Ciphertext();
            for (int l = 1; l < uDiagonals.GetLength(0); l++)
            {
                ctResult.Add(new Ciphertext());
                evaluator.RotateVector(ctNew, l, gal_keys, tempRot);
                evaluator.MultiplyPlain(tempRot, uDiagonals[l], ctResult[l]);
            }
            var ctPrime = new Ciphertext();
            evaluator.AddMany(ctResult, ctPrime);

            return ctPrime;
        }
        public Matrix MultiplyMatrixByNumber(Matrix matrix, double number, ExcelPackage package, int row, int column)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using var context = new SEALContext(_parms);

            using var keygen = new KeyGenerator(context);
            using var publicKey = keygen.PublicKey;
            using var secretKey = keygen.SecretKey;
            using var relinKeys = keygen.RelinKeysLocal();
            using var galKeys = keygen.GaloisKeysLocal();
            using var encryptor = new Encryptor(context, publicKey);
            using var evaluator = new Evaluator(context);
            using var decryptor = new Decryptor(context, secretKey);

            using var ckksEncoder = new CKKSEncoder(context);

            var slotCount = ckksEncoder.SlotCount;
            var cypherCount = (ulong)matrix.Elements.Length / slotCount + 1;
            var square = (ulong)(matrix.RowSize * matrix.RowSize);

            var podMatrix = new List<double[]>((int)cypherCount);
            var elems = matrix.Elements;
            for (ulong j = 0; j < cypherCount; j++)
            {
                podMatrix.Add(new double[slotCount]);
                for (ulong i = 0;
                    i < slotCount && i + j * slotCount < square;
                    i++)
                {
                    podMatrix[(int)j][i] = elems[i + j * slotCount];
                }
            }

            var scale = Math.Pow(2.0, 10);

            var plainMatrix = new Plaintext[cypherCount];
            var plainCoeff = new Plaintext();
            var encryptedMatrix = new Ciphertext[cypherCount];
            for (int j = 0; j < (int)cypherCount; j++)
            {
                plainMatrix[j] = new Plaintext();
                ckksEncoder.Encode(podMatrix[j], scale, plainMatrix[j]);
                encryptedMatrix[j] = new Ciphertext();
                encryptor.Encrypt(plainMatrix[j], encryptedMatrix[j]);
            }
            //evaluator.RelinearizeInplace(encryptedMatrix, relinKeys);
            
            var multiplyBy = number;
            ckksEncoder.Encode(multiplyBy, scale, plainCoeff);

            var bytesBefore = GC.GetTotalMemory(false);
            var multiplyByNumber = new Ciphertext[cypherCount];
            for (int j = 0; j < (int)cypherCount; j++)
            {
                multiplyByNumber[j] = new Ciphertext();
                evaluator.MultiplyPlain(encryptedMatrix[j], plainCoeff, multiplyByNumber[j]);
            }
            //evaluator.RelinearizeInplace(multiplyByNumber, relinKeys);
            var bytesAfter = GC.GetTotalMemory(false);

            var plainResult = new Plaintext[cypherCount];
            var result = new List<double>();

            for (int j = 0; (ulong)j < cypherCount; j++)
            {
                plainResult[j] = new Plaintext();
                decryptor.Decrypt(multiplyByNumber[j], plainResult[j]);
                var temp = new List<double>();
                ckksEncoder.Decode(plainResult[j], temp);
                result.AddRange(temp);
            }

            var resultMatrix = new Matrix()
            {
                Elements = result.ToArray(),
                RowSize = matrix.RowSize
            };

            stopwatch.Stop();
            var sheet = package.Workbook.Worksheets[(int)TableEnum.MulByNumberEncryptOperationTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;
            sheet = package.Workbook.Worksheets[(int)TableEnum.MulByNumberEncryptResultRam];
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

            using var context = new SEALContext(_parms);

            using var keygen = new KeyGenerator(context);
            using var publicKey = keygen.PublicKey;
            using var secretKey = keygen.SecretKey;
            using var relinKeys = keygen.RelinKeysLocal();
            using var galKeys = keygen.GaloisKeysLocal();
            using var encryptor = new Encryptor(context, publicKey);
            using var evaluator = new Evaluator(context);
            using var decryptor = new Decryptor(context, secretKey);

            using var ckksEncoder = new CKKSEncoder(context);

            var slotCount = ckksEncoder.SlotCount;

            var cypherCount = (ulong) a.Elements.Length / slotCount + 1;

            var podMatrix1 = new List<double[]>((int)cypherCount);
            var podMatrix2 = new List<double[]>((int)cypherCount);
            var elems1 = a.Elements;
            var elems2 = b.Elements;
            var square = (ulong)(a.RowSize * a.RowSize);

            for (ulong j = 0; j < cypherCount; j++)
            {
                podMatrix1.Add(new double[slotCount]);
                podMatrix2.Add(new double[slotCount]);
                for (ulong i = 0; 
                    i < slotCount && i + j * slotCount < square;
                    i++)
                {
                    podMatrix1[(int)j][i] = elems1[i + j * slotCount];
                    podMatrix2[(int)j][i] = elems2[i + j * slotCount];
                }
            }

            double scale = Math.Pow(2.0, 40);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Plaintext[] plainMatrix1 = new Plaintext[cypherCount],
                            plainMatrix2 = new Plaintext[cypherCount];

            Ciphertext[] encryptedMatrix1 = new Ciphertext[cypherCount],
                            encryptedMatrix2 = new Ciphertext[cypherCount];
            //TODO: refactor

            for (int j = 0; (ulong)j < cypherCount; j++)
            {
                plainMatrix1[j] = new Plaintext();
                ckksEncoder.Encode(podMatrix1[j], scale, plainMatrix1[j]);
                encryptedMatrix1[j] = new Ciphertext();
                encryptor.Encrypt(plainMatrix1[j], encryptedMatrix1[j]);
                //evaluator.RelinearizeInplace(encryptedMatrix1[j], relinKeys);
            }

            long bytesBefore = 0, bytesAfter = 0;
            bytesBefore = GC.GetTotalMemory(false);
            for (int j = 0; (ulong)j < cypherCount; j++)
            {
                plainMatrix2[j] = new Plaintext();
                ckksEncoder.Encode(podMatrix2[j], scale, plainMatrix2[j]);
                encryptedMatrix2[j] = new Ciphertext();
                encryptor.Encrypt(plainMatrix2[j], encryptedMatrix2[j]);
                
                //evaluator.RelinearizeInplace(encryptedMatrix2[j], relinKeys);
            }

            bytesAfter = GC.GetTotalMemory(false);
            stopwatch.Stop();
            var sheet = package.Workbook.Worksheets[(int)TableEnum.SumEncryptingTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;
            sheet = package.Workbook.Worksheets[(int)TableEnum.SumEncryptingRam];
            var bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            stopwatch.Reset();
            stopwatch.Start();

            for (int j = 0; (ulong) j < cypherCount; j++)
            {
                var testPlain = new Plaintext();
                decryptor.Decrypt(encryptedMatrix2[j], testPlain);
                ckksEncoder.Decode(testPlain, new List<double>());
            }

            stopwatch.Stop();
            sheet = package.Workbook.Worksheets[(int)TableEnum.SumDecryptingTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();

            bytesBefore = GC.GetTotalMemory(false);
            var sumOfMatrix = new Ciphertext[cypherCount];
            for (int j = 0; (ulong) j < cypherCount; j++)
            {
                sumOfMatrix[j] = new Ciphertext();
                evaluator.Add(encryptedMatrix1[j], encryptedMatrix2[j], sumOfMatrix[j]);
            }

            bytesAfter = GC.GetTotalMemory(false);
            //evaluator.RelinearizeInplace(sumOfMatrix, relinKeys);

            var plainResult = new Plaintext[cypherCount];
            var result = new List<double>();
            for (int j = 0; (ulong) j < cypherCount; j++)
            {
                plainResult[j] = new Plaintext();
                decryptor.Decrypt(sumOfMatrix[j], plainResult[j]);
                var temp = new List<double>();
                ckksEncoder.Decode(plainResult[j], temp);
                result.AddRange(temp);
            }

            
           

            var resultMatrix = new Matrix()
            {
                Elements = result.ToArray(),
                RowSize = a.RowSize
            };

            stopwatch.Stop();
            sheet = package.Workbook.Worksheets[(int)TableEnum.SumEncryptOperationTime];
            sheet.Cells[row, column].Value = stopwatch.ElapsedMilliseconds;
            sheet = package.Workbook.Worksheets[(int)TableEnum.SumEncryptResultRam];
            bytes = bytesAfter - bytesBefore;
            sheet.Cells[row, column].Value = bytes;

            return resultMatrix;
        }
    }
}
