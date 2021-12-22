using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionJNZ : IInstruction, IInstruction<ISpecReferences>, IInstruction<ISpecRegisters>
    {
        public string Name => "JNZ";

        public byte OPCode => 17;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecReferences references = (ISpecReferences)compiler;
            ISpecRegisters registers = (ISpecRegisters)compiler;

            if (instruction.Parameters.Count != 2)
            {
                compiler.ThrowError(instruction, "JNZ instruction must have a parameter as the register and a second one indicating the reference to jump into. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (!references.SetReferenceBlock(instruction, instruction.Parameters[1], 2, 4))
            {
                compiler.ThrowError(instruction, "JNZ instruction must have a valid reference name.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[0], out byte register))
            {
                return new byte[] { OPCode, register, 0, 0, 0, 0 };
            }

            compiler.ThrowError(instruction, "JNZ instruction must have a first parameter as register. The first parameter is not a register.");
            return new byte[] { 0 };


        }
    }
}
