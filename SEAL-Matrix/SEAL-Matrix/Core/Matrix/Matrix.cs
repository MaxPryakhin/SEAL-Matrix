using System;
using System.Collections.Generic;
using System.Text;

namespace SEAL_Matrix.Core.Matrix
{
    [Serializable]
    public class Matrix
    {
        public double[] Elements { get; set; }
        public int RowSize { get; set; }
    }
}
