using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public abstract class Instruction3Registers : IInstruction, IInstruction<ISpecRegisters>
    {
        public abstract string Name { get; }
        public abstract byte OPCode { get; }

        public virtual byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;

            if (instruction.Parameters.Count != 3)
            {
                compiler.ThrowError(instruction, Name + " instruction must have 3 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[0], out byte r1) && registers.GetRegister(instruction.Parameters[1], out byte r2) && registers.GetRegister(instruction.Parameters[2], out byte r3))
            {
                return new byte[] { OPCode, r1, r2, r3 };
            }

            compiler.ThrowError(instruction, Name + " instruction must have 3 register parameters.");
            return new byte[] { 0 };

        }
    }
}
