using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.SEAL;

namespace SEAL_Matrix.Core.Matrix
{
    class MatrixHomomorphicStrategy : IMatrixStrategy
    {
        public double[] MultiplyMatrixByNumber(Matrix matrix, double number)
        {
            using EncryptionParameters parms = new EncryptionParameters(SchemeType.CKKS);

            ulong polyModulusDegree = 8192;
            parms.PolyModulusDegree = polyModulusDegree;
            parms.CoeffModulus = CoeffModulus.Create(
                polyModulusDegree, new int[] { 40, 40, 40, 40, 40 });

            using SEALContext context = new SEALContext(parms);

            using KeyGenerator keygen = new KeyGenerator(context);
            using PublicKey publicKey = keygen.PublicKey;
            using SecretKey secretKey = keygen.SecretKey;
            using RelinKeys relinKeys = keygen.RelinKeysLocal();
            using GaloisKeys galKeys = keygen.GaloisKeysLocal();
            using Encryptor encryptor = new Encryptor(context, publicKey);
            using Evaluator evaluator = new Evaluator(context);
            using Decryptor decryptor = new Decryptor(context, secretKey);

            using CKKSEncoder ckksEncoder = new CKKSEncoder(context);

            ulong slotCount = ckksEncoder.SlotCount;

            double[] podMatrix = new double[slotCount];
            var elems = matrix.Elements;
            for (int i = 0; i < elems.Length; i++)
            {
                podMatrix[i] = elems[i];
            }

            double scale = Math.Pow(2.0, 40);

            using Plaintext plainMatrix = new Plaintext(),
                            plainCoeff = new Plaintext();
            ckksEncoder.Encode(podMatrix, scale, plainMatrix);
            using Ciphertext encryptedMatrix = new Ciphertext();
            encryptor.Encrypt(plainMatrix, encryptedMatrix);
            evaluator.RelinearizeInplace(encryptedMatrix, relinKeys);
            
            var multiplyBy = number;
            ckksEncoder.Encode(multiplyBy, scale, plainCoeff);

            using Ciphertext multiplyByNumber = new Ciphertext();
            evaluator.MultiplyPlain(encryptedMatrix, plainCoeff, multiplyByNumber);
            evaluator.RelinearizeInplace(multiplyByNumber, relinKeys);

            using Plaintext plainResult = new Plaintext();
            decryptor.Decrypt(multiplyByNumber, plainResult);
            var result = new List<double>();
            ckksEncoder.Decode(plainResult, result);

            return result.ToArray();
        }

        public double[] SumMatrix(Matrix a, Matrix b)
        {
            throw new NotImplementedException();
        }
    }
}
