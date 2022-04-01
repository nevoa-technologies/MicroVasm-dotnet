using MicroVASMDotNET;
using System.IO;

namespace MicroVASMDotNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "main.vasm";

            if (args.Length > 0)
            {
                if (args.Length == 1)
                    fileName = args[0];
                else
                {
                    System.Console.WriteLine("Received {0} arguments. It can only receive one file.", args.Length);
                    return;
                }
            }

            if (!File.Exists(fileName))
            {
                System.Console.WriteLine("File '{0}' not found.", fileName);
                return;
            }

            string sourceCode = File.ReadAllText(fileName);

            using (var data = MicroVASM.Prepare(sourceCode))
            {
                CompilerResult result = MicroVASM.Compile(data, MicroVASMVersion.V1_0, true);
                File.WriteAllBytes("result.bin", result.Result);
            }
        }
    }
}
