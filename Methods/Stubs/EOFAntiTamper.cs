using System;
using System.IO;
using System.Security.Cryptography;

namespace Sharkfuscator.Methods.Stubs
{
    class EOFAntiTamper
    {
        static void Initialize()
        {
            int len = 16;
            string this_md5 = string.Empty;
            string stored_md5 = string.Empty;
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(System.Reflection.Assembly.GetExecutingAssembly().Location)))
            {
                ms.Seek(-len, SeekOrigin.End);
                byte[] stored_md5_bytes = new byte[len];
                ms.Read(stored_md5_bytes, 0, len);
                stored_md5 = BitConverter.ToString(stored_md5_bytes);

                ms.Position = 0;
                byte[] this_md5_bytes = new byte[ms.Length - len];
                ms.Read(this_md5_bytes, 0, this_md5_bytes.Length);
                this_md5 = BitConverter.ToString(MD5.Create().ComputeHash(this_md5_bytes));
            }
            if(stored_md5 != this_md5)
            {
                // System.Windows.Forms.MessageBox.Show(string.Format("Invalid!\r\nStored md5: {0}\r\nThis md5: {1}", stored_md5, this_md5));
                throw new BadImageFormatException(null);
            }
        }
    }
}
