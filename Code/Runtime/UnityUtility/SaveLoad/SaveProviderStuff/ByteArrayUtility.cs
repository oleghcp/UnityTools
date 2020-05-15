using System;
using System.Collections.Generic;

namespace UnityUtility.SaveLoad.SaveProviderStuff
{
    internal static class ByteArrayUtility
    {
        public static string ToString(byte[] array)
        {
            return array.ConcatToString(".");
        }

        public static byte[] FromString(string serialized)
        {
            string[] splitted = serialized.Split('.');
            byte[] bytes = new byte[splitted.Length];

            for (int i = 0; i < splitted.Length; i++)
            {
                if (byte.TryParse(splitted[i], out bytes[i]))
                {
                    continue;
                }

                throw new InvalidOperationException("Not a byte array.");
            }

            return bytes;
        }
    }
}
