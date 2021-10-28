using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionNOT : IInstruction, IInstruction<ISpecRegisters>
    {
        public string Name => "NOT";
        public byte OPCode => 20;

        public virtual byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;

            if (instruction.Parameters.Count != 2)
            {
                compiler.ThrowError(instruction, Name + " instruction must have 2 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[0], out byte r1) && registers.GetRegister(instruction.Parameters[1], out byte r2))
            {
                return new byte[] { OPCode, r1, r2 };
            }

            compiler.ThrowError(instruction, Name + " instruction must have 2 register parameters.");
            return new byte[] { 0 };

        }
    }
}
