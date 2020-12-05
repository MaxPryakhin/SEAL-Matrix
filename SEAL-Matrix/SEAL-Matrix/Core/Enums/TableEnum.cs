using System.ComponentModel;

namespace SEAL_Matrix.Core.Enums
{
    public enum TableEnum
    {
        [Description("0.Параметры криптосистемы")]
        Params = 0,

        [Description("1.Объем ОЗУ для хранения матриц")]
        Ram = 1,
        [Description("1.Время шифрования матриц (сумма)")]
        SumEncryptingTime = 2,
        [Description("1.Объем ОЗУ для хранения зашифрованных  матриц (сумма)")]
        SumEncryptingRam = 3,
        [Description("1.Время дешифрования матриц (сумма)")]
        SumDecryptingTime = 4,
        [Description("1.Время шифрования матриц (умножение)")]
        MulEncryptingTime = 5,
        [Description("1.Объем ОЗУ для хранения зашифрованных  матриц (умножение)")]
        MulEncryptingRam = 6,
        [Description("1.Время дешифрования матриц (умножение)")]
        MulDecryptingTime = 7,

        [Description("2.Время сложения матриц")]
        SumTime = 7,
        [Description("2.Объем ОЗУ для хранения матрицы сложения")]
        SumRam = 8,
        [Description("2.Время сложения зашифрованных матриц")]
        SumEncryptOperationTime = 9,
        [Description("2.Объем ОЗУ для хранения зашифрованной матрицы сложения")]
        SumEncryptResultRam = 10,
        [Description("2.Ошибка результата")]
        SumResultError = 11,

        [Description("3.Время умножения матрицы на число")]
        MulByNumberTime = 12,
        [Description("3.Объем ОЗУ для хранения матрицы, умноженной на число")]
        MulByNumberRam = 13,
        [Description("3.Время умноженния зашифрованной матрицы на число")]
        MulByNumberEncryptOperationTime = 14,
        [Description("3.Объем ОЗУ для хранения зашифрованной матрицы, умноженной на число")]
        MulByNumberEncryptResultRam = 15,
        [Description("3.Ошибка результата")]
        MulByNumberResultError = 16,

        [Description("4.Время умножения матриц")]
        MulTime = 17,
        [Description("4.Объем ОЗУ для хранения матрицы умножения")]
        MulRam = 18,
        [Description("4.Время умножения зашифрованных матриц")]
        MulEncryptOperationTime = 19,
        [Description("4.Объем ОЗУ для хранения зашифрованной матрицы умножения")]
        MulEncryptResultRam = 20,
        [Description("4.Ошибка результата")]
        MulResultError = 21,
    }
}

