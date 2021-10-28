using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionCMP : IInstruction, IInstruction<ISpecRegisters>, IInstruction<ISpecComparsions>
    {
        public string Name => "CMP";
        public byte OPCode => 15;

        public virtual byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;
            ISpecComparsions comparsions = (ISpecComparsions)compiler;


            if (instruction.Parameters.Count != 4)
            {
                compiler.ThrowError(instruction, Name + " instruction must have 4 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            byte cmp = 0;

            if (!comparsions.GetComparsionCode(instruction.Parameters[0], out cmp))
            {
                compiler.ThrowError(instruction, Name + " has an invalid comparsion. " + instruction.Parameters[0] + " is not a comparsion type.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[1], out byte r1) && registers.GetRegister(instruction.Parameters[2], out byte r2) && registers.GetRegister(instruction.Parameters[3], out byte r3))
            {
                return new byte[] { OPCode, cmp, r1, r2, r3 };
            }

            compiler.ThrowError(instruction, Name + " instruction must have 3 register parameters after the comparsion type.");
            return new byte[] { 0 };
        }
    }
}
