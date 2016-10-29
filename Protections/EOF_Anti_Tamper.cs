using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using Sharkfuscator.ConfuserEx;
using Sharkfuscator.Protections.Stubs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Sharkfuscator.Protections
{
    /*
     * Credits: XenoCodeRCE
     * Source: https://github.com/XenocodeRCE/AntiTamperEOF
     */
    public class EOF_Anti_Tamper : iProtection
    {
        public string description
        {
            get
            {
                return "Anti-tamper protection coded by XenoCodeRCE. Adds hash to EOF and compares it on run.";
            }
        }

        public string name
        {
            get
            {
                return "EOF Anti-Tamper";
            }
        }

        public string author
        {
            get
            {
                return "XenoCodeRCE";
            }
        }

        public string init_message
        {
            get
            {
                return "Injecting anti-tamper class..";
            }
        }

        public void Protect(Stream stream)
        {
            InjectAntiTamper(stream);
        }

        private void InjectAntiTamper(Stream stream)
        {
            ModuleDefMD mod = ModuleDefMD.Load(stream);
            AddCall(mod);
            var opts = new ModuleWriterOptions(mod);
            opts.Logger = DummyLogger.NoThrowInstance;

            stream.Position = 0;
            mod.Write(stream);

            //byte[] hash = stream.CalculateHash();
            //stream.Seek(0L, SeekOrigin.End);
            //stream.Write(hash, 0, hash.Length);

            stream.Position = 0;
        }

        public static void InjectHash(Stream stream)
        {
            byte[] hash = stream.CalculateHash();
            stream.Seek(0L, SeekOrigin.End);
            stream.Write(hash, 0, hash.Length);
            stream.Position = 0;
        }

        public static void InjectHash(string filename)
        {
            //We get the md5 as byte, of the target
            byte[] md5bytes = System.Security.Cryptography.MD5.Create().ComputeHash(System.IO.File.ReadAllBytes(filename));
            //Let's use FileStream to edit the file's byte
            using (var stream = new FileStream(filename, FileMode.Append))
            {
                //Append md5 in the end
                stream.Write(md5bytes, 0, md5bytes.Length);
            }
        }

        private void AddCall(ModuleDef module)
        {
            //We declare our Module, here we want to load the EOFAntitamp class, from AntiTamperEOF.exe
            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(EOFAntiTamper).Module);
            //We find or create the .cctor method in <Module>, aka GlobalType, if it doesn't exist yet
            MethodDef cctor = module.GlobalType.FindOrCreateStaticConstructor();
            //We declare EOFAntitamp as a TypeDef using it's Metadata token (needed)
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(EOFAntiTamper).MetadataToken));
            //We use confuserEX InjectHelper class to inject EOFAntitamp class into our target, under <Module>
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, module.GlobalType, module);

            //We find the Initialize() Method in EOFAntitamp we just injected
            var init = (MethodDef)members.Single(method => method.Name == "Initialize");
            //We call this method using the Call Opcode
            cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));


            //We just have to remove .ctor method because otherwise it will
            //lead to Global constructor error (e.g [MD]: Error: Global item (field,method) must be Static. [token:0x06000002] / [MD]: Error: Global constructor. [token:0x06000002] )
            foreach (MethodDef md in module.GlobalType.Methods)
            {
                if (md.Name == ".ctor")
                {
                    module.GlobalType.Remove(md);
                    //Now we go out of this mess
                    break;
                }
            }
        }
    }
}
