using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionJMP : IInstruction, IInstruction<ISpecReferences>, IInstruction<ISpecRegisters>
    {
        public string Name => "JMP";

        public byte OPCode => 16;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecReferences references = (ISpecReferences)compiler;

            if (instruction.Parameters.Count != 1)
            {
                compiler.ThrowError(instruction, "JMP instruction must have a parameter indicating the reference to jump into. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (!references.SetReferenceBlock(instruction, instruction.Parameters[0], 1, 4))
            {
                compiler.ThrowError(instruction, "JNZ instruction must have a valid reference name.");
                return new byte[] { 0 };
            }

            return new byte[] { OPCode, 0, 0, 0, 0 };
        }
    }
}
