using MicroVASMDotNET.Compilers;
using System;
using System.Collections.Generic;

namespace MicroVASMDotNET
{
    public static class MicroVASM
    {
        public static VASMCodeData Prepare(string vasmSourceCode)
        {
            List<VASMInstructionData> instructions = new List<VASMInstructionData>();

            string[] lines = vasmSourceCode.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                List<string> fields = new List<string>();
                string field = "";

                for (int j = 0; j < line.Length; j++)
                {
                    char c = line[j];

                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == ';')
                    {
                        if (field != "")
                            fields.Add(field);

                        field = "";

                        if (c == ';')
                            break;

                        continue;
                    }

                    if (c == '\'')
                    {
                        field += c;
                        j++;
                        while (j < line.Length && line[j] != '\'')
                        {
                            field += line[j];
                            j++;
                        }
                    }


                    if (c == '\"')
                    {
                        field += c;
                        j++;
                        while (j < line.Length && line[j] != '\"')
                        {
                            field += line[j];
                            j++;
                        }
                    }

                    field += line[j];

                    // Check if it is the last character, othwerise it wouldn't add the last field
                    // unless the line has at least a space at the end.
                    if (j == line.Length - 1)
                        fields.Add(field);
                }

                if (fields.Count > 0)
                {
                    string instruction = fields[0];
                    fields.RemoveAt(0);

                    instructions.Add(new VASMInstructionData(instruction, fields, i));
                }
            }

            return new VASMCodeData(instructions);
        }


        public static CompilerResult Compile(VASMCodeData codeData, MicroVASMVersion microVASMVersion, bool is64Bit)
        {
            MicroVASMBaseCompiler compiler = CreateCompiler(microVASMVersion, is64Bit);

            compiler.StartPreProcessing();

            foreach (VASMInstructionData instruction in codeData.instructions)
                compiler.NextInstruction(instruction);

            compiler.StopPreProcessing();

            foreach (VASMInstructionData instruction in codeData.instructions)
                compiler.NextInstruction(instruction);

            return compiler.Generate();
        }


        private static MicroVASMBaseCompiler CreateCompiler(MicroVASMVersion microVASMVersion, bool is64Bit)
        {
            switch (microVASMVersion)
            {
                case MicroVASMVersion.V1_0:
                    return new MicroVASMCompiler_1_0(is64Bit);
                default:
                    return null;
            }
        }
    }
}
