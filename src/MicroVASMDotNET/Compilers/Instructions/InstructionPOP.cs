using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionPOP : IInstruction, IInstruction<ISpecRegisters>, IInstruction<ISpecTypes>, IInstruction<ISpecReferences>
    {
        public string Name => "POP";

        public byte OPCode => 65;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;
            ISpecTypes types = (ISpecTypes)compiler;

            if (instruction.Parameters.Count != 2)
            {
                compiler.ThrowError(instruction, "POP instruction must have 2 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            byte register;
            int size;

            if (!registers.GetRegister(instruction.Parameters[0], out register))
            {
                compiler.ThrowError(instruction, "PUSH instruction must have a register in the first parameter. The first parameter is not a register.");
                return new byte[] { 0 };
            }

            if (!types.GetTypeSize(instruction.Parameters[1], out size))
            {
                compiler.ThrowError(instruction, "POP instruction must have a type in the third parameter. The third parameter is not a type or a size.");
                return new byte[] { 0 };
            }

            if (size > compiler.MaxTypeSize)
            {
                compiler.ThrowError(instruction, "POP instruction must have a type with a size not bigger than " + compiler.MaxTypeSize + ".");
                return new byte[] { 0 };
            }

            List<byte> result = new List<byte>();
            result.Add(OPCode);
            result.Add(register);
            result.Add((byte)size);

            return result.ToArray();
        }
    }
}

