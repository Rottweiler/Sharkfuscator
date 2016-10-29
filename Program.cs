using Fclp;
using Sharkfuscator.Protections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharkfuscator
{

    /// <summary>
    /// For command-line arguments
    /// </summary>
    public class ApplicationArguments
    {
        public string input_file { get; set; }
        public string output_file { get; set; }
        public bool strip_dos_header { get; set; }
        public bool eof_anti_tamper { get; set; }
    }

    /// <summary>
    /// Parameter management
    /// </summary>
    class Program
    {
        static List<iProtection> protections = new List<iProtection>();

        static void Main(string[] args)
        {
            PrintHello();

            var parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.Setup(arg => arg.input_file)
             .As('i', "input")
             .Required();

            parser.Setup(arg => arg.output_file)
             .As('o', "output");

            parser.Setup(arg => arg.strip_dos_header)
             .As('d', "strip-dos-header")
             .SetDefault(true);

            parser.Setup(arg => arg.eof_anti_tamper)
             .As('e', "eof-anti-tamper")
             .SetDefault(true);

            var result = parser.Parse(args);

            if (result.HasErrors == false)
            {
                App(parser.Object);
            }else
            {
                PrintHelp();
            }
        }

        /// <summary>
        /// Main program functionality
        /// </summary>
        /// <param name="o"></param>
        static void App(object o)
        {
            var arguments = (ApplicationArguments)o;

            if (arguments.output_file == string.Empty || arguments.output_file == null)
                arguments.output_file = arguments.input_file + ".shark.exe";

            using (MemoryStream base_stream = new MemoryStream())
            using (FileStream fs = new FileStream(arguments.input_file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.CopyTo(base_stream);

                /*
                 * Perform protections
                 */
                if (arguments.strip_dos_header)
                    protections.Add(new DOSModifier());
                if (arguments.eof_anti_tamper)
                    protections.Add(new EOF_Anti_Tamper());

                foreach (var protection in protections)
                {
                    Console.WriteLine(protection.init_message);
                    protection.Protect(base_stream);
                }

                /*
                 * Write output
                 */
                Console.WriteLine("Writing " + arguments.output_file + "..");
                if (File.Exists(arguments.output_file)) File.Delete(arguments.output_file);
                using (FileStream fs_out = new FileStream(arguments.output_file, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    base_stream.CopyTo(fs_out);
                    fs_out.Flush();
                }

                if (arguments.eof_anti_tamper)
                {
                    Console.WriteLine("Appending final anti-tamper hash..");
                    EOF_Anti_Tamper.InjectHash(arguments.output_file);
                }

                Console.WriteLine("Done");
            }
        }

        /// <summary>
        /// Print hello
        /// </summary>
        static void PrintHello()
        {
            Console.Clear();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" ____  _                _     __                     _             ");
            sb.AppendLine("/ ___|| |__   __ _ _ __| | __/ _|_   _ ___  ___ __ _| |_ ___  _ __ ");
            sb.AppendLine("\\___ \\| '_ \\ / _` | '__| |/ / |_| | | / __|/ __/ _` | __/ _ \\| '__|");
            sb.AppendLine(" ___) | | | | (_| | |  |   <|  _| |_| \\__ \\ (_| (_| | || (_) | |   ");
            sb.AppendLine("|____/|_| |_|\\__,_|_|  |_|\\_\\_|  \\__,_|___/\\___\\__,_|\\__\\___/|_|   ");
            sb.AppendLine("                                                                   ");
            sb.AppendLine("Project repository can be found at https://github.com/Rottweiler/Sharkfuscator/");
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Print help
        /// </summary>
        static void PrintHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Invalid usage!!1");
            sb.AppendLine("Here is how you do it, scrub: ./Sharkfuscator.exe -i INPUT_FILE.exe -o OUTPUT_FILE.exe");
            Console.WriteLine(sb.ToString());
        }
    }
}
