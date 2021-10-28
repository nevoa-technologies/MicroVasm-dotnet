using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroVASMDotNET.Compilers.PreProcessing
{
    public class PreProcessorFunction : IPreProcessor
    {
        private HashSet<string> functions = new HashSet<string>();

        private List<string> builtFunctionNames;

        protected byte OPcodeINVOKE => 8;


        public void PreProcessInstruction(ICompiler compiler, VASMInstructionData instruction)
        {
            if (instruction.Instruction == "FUNCTION")
            {
                if (instruction.Parameters.Count == 0)
                {
                    compiler.ThrowError(instruction, "FUNCTION must have a name.");
                    return;
                }
                else if (instruction.Parameters.Count > 1)
                {
                    compiler.ThrowError(instruction, "FUNCTION must have only one parameter.");
                    return;
                }

                string functionName = instruction.Parameters[0];

                if (functions.Contains(functionName))
                {
                    compiler.ThrowError(instruction, "FUNCTION already declared.");
                    return;
                }

                functions.Add(functionName);
            }
        }


        public void StartProcessing(ICompiler compiler)
        {
            
        }


        public byte[] ProcessInstruction(ICompiler compiler, VASMInstructionData instruction)
        {
            if (instruction.Instruction == "FUNCTION")
                return new byte[0];

            if (instruction.Instruction == "INVOKE")
            {
                if (!instruction.HasParameters)
                {
                    compiler.ThrowError(instruction, "INVOKE must have the name of the function to be invoked.");
                    return new byte[0];
                }

                if (instruction.Parameters.Count > 1)
                {
                    compiler.ThrowError(instruction, "INVOKE can only have one parameter indicating the name of the function.");
                    return new byte[0];
                }

                if (!builtFunctionNames.Contains(instruction.Parameters[0]))
                {
                    compiler.ThrowError(instruction, "INVOKE cannot have an undefined function name.");
                    return new byte[0];
                }

                short functionIndex = (short) builtFunctionNames.IndexOf(instruction.Parameters[0]);
                byte[] indexBytes = BitConverter.GetBytes(functionIndex).AsLittleEndian();

                List<byte> result = new List<byte>();
                result.Add(OPcodeINVOKE);
                result.AddRange(indexBytes);

                return result.ToArray();
            }

            return null;
        }


        public string[] BuildFunctionNamesArray()
        {
            builtFunctionNames = new List<string>();
            builtFunctionNames.AddRange(functions);
            return builtFunctionNames.ToArray();
        }


        public bool HasFunction(string name)
        {
            return functions.Contains(name);
        }
    }
}
