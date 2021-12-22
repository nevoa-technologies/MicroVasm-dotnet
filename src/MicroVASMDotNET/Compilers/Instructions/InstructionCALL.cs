using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    class InstructionCALL : IInstruction, IInstruction<ISpecReferences>
    {
        public string Name => "CALL";

        public byte OPCode => 18;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecReferences references = (ISpecReferences)compiler;

            if (instruction.Parameters.Count != 1)
            {
                compiler.ThrowError(instruction, "CALL instruction must have one parameter indicating the name. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (!references.SetReferenceBlock(instruction, instruction.Parameters[0], 1, 4))
            {
                compiler.ThrowError(instruction, "CALL instruction must have a valid reference name.");
                return new byte[] { 0 };
            }

            return new byte[] { OPCode, 0, 0, 0, 0 };
        }
    }
}
