using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sharkfuscator.Methods
{
    public static class DOS_Stripper
    {
        public static void StripDOSHeader(Stream stream)
        {
            byte[] blank_dos = new byte[64];
            byte[] magic = ReadArray(offset_magic, length_magic, stream);
            byte[] lfanew = ReadArray(offset_lfanew, length_lfanew, stream);

            stream.Position = 0;
            WriteArray(0, blank_dos, stream);
            WriteArray(offset_magic, magic, stream);
            WriteArray(offset_lfanew, lfanew, stream);
            WriteArray(0x4e, new byte[39], stream); //override This program can not be run in DOS mode.

            //reset stream position
            stream.Position = 0;

            //Console.WriteLine("magic: " + magic.ToHex());
            //Console.WriteLine("lfanew: " + lfanew.ToHex());
        }

        private static byte[] ReadArray(UInt32 offset, int length, Stream stream)
        {
            byte[] data = new byte[length];
            stream.Position = offset;
            stream.Read(data, 0, data.Length);
            return data;
        }

        private static int WriteArray(UInt32 offset, byte[] data, Stream stream)
        {
            stream.Position = offset;
            stream.Write(data, 0, data.Length);
            return data.Length;
        }

        private static UInt32 offset_lfanew = 0x3C;
        private static int length_lfanew = sizeof(UInt32);

        private static UInt32 offset_magic = 0x00;
        private static int length_magic = sizeof(UInt16);
    }
}
