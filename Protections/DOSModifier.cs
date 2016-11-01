using System;
using System.IO;
using Plugin;

namespace Sharkfuscator.Protections
{
    public class DOSModifier : iProtection
    {
        public string name
        {
            get { return "MS-DOS Header remover"; }
        }

        public string description
        {
            get { return "Strips the MS-DOS header and the little text from the executable."; }
        }

        public string author
        {
            get { return "Rottweiler"; }
        }

        public string init_message
        {
            get { return "Stripping DOS header.."; }
        }

        public char command_short
        {
            get { return 'd'; }
        }

        public string command_long
        {
            get { return "strip-dos"; }
        }

        private bool _enabled;
        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool enabled_default
        {
            get { return false; }
        }

        public bool Protect(ProtectorState state, string output_filename, Stream stream)
        {
            if (state == ProtectorState.During)
            {
                StripDOSHeader(stream);
                return true;
            }
            return false;
        }

        private void StripDOSHeader(Stream stream)
        {
            byte[] blank_dos = new byte[64];
            byte[] magic = ReadArray(offset_magic, length_magic, stream);
            byte[] lfanew = ReadArray(offset_lfanew, length_lfanew, stream);

            stream.Position = 0;
            WriteArray(0, blank_dos, stream);
            WriteArray(offset_magic, magic, stream);
            WriteArray(offset_lfanew, lfanew, stream);
            WriteArray(0x4e, new byte[39], stream); //override This program can not be run in DOS mode.
        }

        private byte[] ReadArray(UInt32 offset, int length, Stream stream)
        {
            byte[] data = new byte[length];
            stream.Position = offset;
            stream.Read(data, 0, data.Length);
            return data;
        }

        private int WriteArray(UInt32 offset, byte[] data, Stream stream)
        {
            stream.Position = offset;
            stream.Write(data, 0, data.Length);
            return data.Length;
        }

        private UInt32 offset_lfanew = 0x3C;
        private int length_lfanew = sizeof(UInt32);

        private UInt32 offset_magic = 0x00;
        private int length_magic = sizeof(UInt16);
    }
}
