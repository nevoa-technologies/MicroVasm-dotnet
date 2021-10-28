using MicroVASMDotNET;
using System.IO;

namespace MicroVASMDotNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceCode = File.ReadAllText("main.vasm");

            using (var data = MicroVASM.Prepare(sourceCode))
            {
                byte[] result = MicroVASM.Compile(data, MicroVASMVersion.V1_0);
                File.WriteAllBytes("result.bin", result);
            }
        }
    }
}
