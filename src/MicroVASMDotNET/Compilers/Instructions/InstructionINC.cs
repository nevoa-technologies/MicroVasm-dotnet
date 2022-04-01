using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionINC : IInstruction, IInstruction<ISpecRegisters>
    {
        public string Name => "INC";
        public byte OPCode => 25;

        public virtual byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;

            if (instruction.Parameters.Count != 1)
            {
                compiler.ThrowError(instruction, Name + " instruction can only have one parameter. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[0], out byte r1))
            {
                return new byte[] { OPCode, r1 };
            }

            compiler.ThrowError(instruction, Name + " instruction must have a register parameter.");
            return new byte[] { 0 };

        }
    }
}
