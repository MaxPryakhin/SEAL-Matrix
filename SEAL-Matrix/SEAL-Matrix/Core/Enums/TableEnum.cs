using System.ComponentModel;

namespace SEAL_Matrix.Core.Enums
{
    public enum TableEnum
    {
        [Description("0.Параметры криптосистемы")]
        Params = 1,

        [Description("1.Объем ОЗУ для хранения матриц")]
        Ram,
        [Description("1.Время шифрования матриц (сумма)")]
        SumEncryptingTime,
        [Description("1.Объем ОЗУ для хранения зашифрованных  матриц (сумма)")]
        SumEncryptingRam,
        [Description("1.Время дешифрования матриц (сумма)")]
        SumDecryptingTime,
        [Description("1.Время шифрования матриц (умножение)")]
        MulEncryptingTime,
        [Description("1.Объем ОЗУ для хранения зашифрованных  матриц (умножение)")]
        MulEncryptingRam,
        [Description("1.Время дешифрования матриц (умножение)")]
        MulDecryptingTime,

        [Description("2.Время сложения матриц")]
        SumTime,
        [Description("2.Объем ОЗУ для хранения матрицы сложения")]
        SumRam,
        [Description("2.Время сложения зашифрованных матриц")]
        SumEncryptOperationTime,
        [Description("2.Объем ОЗУ для хранения зашифрованной матрицы сложения")]
        SumEncryptResultRam,
        [Description("2.Ошибка результата")]
        SumResultError,

        [Description("3.Время умножения матрицы на число")]
        MulByNumberTime,
        [Description("3.Объем ОЗУ для хранения матрицы, умноженной на число")]
        MulByNumberRam,
        [Description("3.Время умноженния зашифрованной матрицы на число")]
        MulByNumberEncryptOperationTime,
        [Description("3.Объем ОЗУ для хранения зашифрованной матрицы, умноженной на число")]
        MulByNumberEncryptResultRam,
        [Description("3.Ошибка результата")]
        MulByNumberResultError,

        [Description("4.Время умножения матриц")]
        MulTime,
        [Description("4.Объем ОЗУ для хранения матрицы умножения")]
        MulRam,
        [Description("4.Время умножения зашифрованных матриц")]
        MulEncryptOperationTime,
        [Description("4.Объем ОЗУ для хранения зашифрованной матрицы умножения")]
        MulEncryptResultRam,
        [Description("4.Ошибка результата")]
        MulResultError,
    }
}

