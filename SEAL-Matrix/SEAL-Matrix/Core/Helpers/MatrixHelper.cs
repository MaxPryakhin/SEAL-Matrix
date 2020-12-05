using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEAL_Matrix.Core.Helpers
{
    public static class MatrixHelper
    {
        public static double[,] GetUSigma(double[,] U)
        {
            var dimension = U.GetLength(0);
            var square = dimension * dimension;
            var uSigma = new double[square, square];

            var k = 0;
            var sigmaRow = 0;
            for (var offset = 0; offset < square; offset += dimension)
            {
                var oneMatrix = GetMatrixOfOnes(k, U);
                for (var oneMatrixIndex = 0; oneMatrixIndex < dimension; oneMatrixIndex++)
                {
                    var tempFill = PadZero(offset, GetRow(oneMatrixIndex, oneMatrix));

                    SetRow(sigmaRow, uSigma, tempFill);

                    sigmaRow++;
                }

                k++;
            }

            return uSigma;
        }

        public static double [,] GetUTau(double[,] U)
        {
            var dimension = U.GetLength(0);
            var square = dimension * dimension;
            var uTau = new double[square, square];

            var tauRow = 0;
            for (var i = 0; i < dimension; i++)
            {
                var oneMatrix = GetMatrixOfOnes(0, U);
                var offset = i * dimension;

                for (var j = 0; j < dimension; j++)
                {
                    var tempFill = PadZero(offset, GetRow(j, oneMatrix));
                    SetRow(tauRow, uTau, tempFill);
                    tauRow++;
                    if (offset + dimension == square)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset += dimension;
                    }
                }
            }

            return uTau;
        }

        public static double [,] GetVk(double[,] u, int k)
        {

            var length = u.GetLength(0);
            var square = length * length;
            var vk = new double[square, square];

            var vRow = 0;
            for (var offset = 0; offset < square; offset += length)
            {
                var oneMatrix = GetMatrixOfOnes(k, u);
                for (var oneMatrixIndex = 0; oneMatrixIndex < length; oneMatrixIndex++)
                {
                    var tempFill = PadZero(offset, GetRow(oneMatrixIndex, oneMatrix));
                    SetRow(vRow, vk, tempFill);
                    vRow++;
                }
            }

            return vk;
        }

        public static double [,] GetWk(double[,] u, int k)
        {

            var length = u.GetLength(0);
            var square = length * length;
            var wk = new double[square, square];

            var wRow = 0;
            var oneMatrix = GetMatrixOfOnes(0, u);
            var offset = k * length;

            for (var i = 0; i < length; i++)
            {
                for (var oneMatrixIndex = 0; oneMatrixIndex < length; oneMatrixIndex++)
                {
                    var tempFill = PadZero(offset, GetRow(oneMatrixIndex, oneMatrix));
                    SetRow(wRow, wk, tempFill);
                    wRow++;
                }
                if (offset + length == square)
                {
                    offset = 0;
                }
                else
                {
                    offset += length;
                }
            }

            return wk;
        }

        public static double[] GetRow(int position, double[,] U)
        {
            var length = U.GetLength(1);
            var array = new double[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = U[position, i];
            }
            return array;
        }

        public static void SetRow(int position, double[,] u, double[] value)
        {
            var length = u.GetLength(1);
            for (var i = 0; i < length; i++)
            {
                u[position, i] = value[i];
            }
        }

        public static double[,] GetMatrixOfOnes(int position, double[,] u)
        {
            var diagonalOfOnes = new double[u.GetLength(0), u.GetLength(1)];
            var U_diag = GetDiagonal(position, u);

            int k = 0;
            for (int i = 0; i < u.GetLength(0); i++)
            {
                for (int j = 0; j < u.GetLength(0); j++)
                {
                    //TODO: possible loss of precision
                    if (u[i,j] == U_diag[k])
                    {
                        diagonalOfOnes[i,j] = 1;
                    }
                    else
                    {
                        diagonalOfOnes[i,j] = 0;
                    }
                }
                k++;
            }

            return diagonalOfOnes;
        }

        public static double[] GetDiagonal(int position, double[,] u)
        {
            var length = u.GetLength(0);
            var diagonal = new double[length];
            var k = 0;
            for (int i = 0, j = position; (i < length - position) && (j < length); i++, j++)
            {
                diagonal[k] = u[i,j];
                k++;
            }
            for (int i = length - position, j = 0; (i < length) && (j < position); i++, j++)
            {
                diagonal[k] = u[i,j];
                k++;
            }

            return diagonal;
        }

        public static List<double[]> GetAllDiagonals(double[,] u)
        {
            var length = u.GetLength(0);
            var diagonalMatrix  = new List<double[]>(length);

            for (var i = 0; i < length; i++)
            {
                diagonalMatrix.Add(GetDiagonal(i, u));
            }

            return diagonalMatrix;
        }

        public static double[] PadZero(int offset, double[] uVec)
        {
            var length = uVec.GetLength(0);
            var resultVec = new double[length * length];

            for (var i = 0; i < offset; i++)
            {
                resultVec[i] = 0;
            }

            for (var i = 0; i < length; i++)
            {
                resultVec[i + offset] = uVec[i];
            }

            for (var i = offset + length; i < resultVec.Length; i++)
            {
                resultVec[i] = 0;
            }
            return resultVec;
        }

        public static Matrix.Matrix GenerateRandomMatrix(int size)
        {
            var elements = new double[size * size];
            var generator = new Random();

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    elements[i * size + j] = generator.NextDouble();
                }
            }


            return new Matrix.Matrix()
            {
                RowSize = size,
                Elements = elements
            };
        }
    }
}
