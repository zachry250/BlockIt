using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockIt.Core.Helper
{
    public static class ByteHelper
    {
        public static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] destinationArray = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, destinationArray, offset, array.Length);
                offset += array.Length;
            }
            return destinationArray;
        }
    }
}
