using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SEAL_Matrix.Core.Helpers
{
    public static class BinaryHelper
    {
        public static void GetByteSize(object obj)
        {
            Marshal.SizeOf(obj);


            //return obj == null ? 0 : sizeof(obj);


            //object Value = null;
            //int size = 0;
            //Type type = obj.GetType();
            //PropertyInfo[] info = type.GetProperties();
            //foreach (PropertyInfo property in info)
            //{
            //    Value = property.GetValue(obj, null);
            //    unsafe
            //    {
            //        size += sizeof(Value);
            //    }
            //}
            //return size;
        }
    }
}
