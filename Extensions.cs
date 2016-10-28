using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sharkfuscator
{
    public static class Extensions
    {
        public static string ToHex(this byte[] main)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte raw_byte in main)
                sb.Append(raw_byte.ToString("x2"));
            return sb.ToString();
        }

        public static void Zero(this byte[] main)
        {
            for (int i = 0; i < main.Length; i++)
                main[i] = 0;
        }

        public static byte[] CalculateHash(this Stream main)
        {
           byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(main);
            Console.WriteLine("Hash: " + hash.ToHex());
            return hash;
        }
    }
}
