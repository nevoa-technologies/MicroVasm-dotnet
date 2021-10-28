using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    class InstructionLDS : IInstruction, IInstruction<ISpecRegisters>, IInstruction<ISpecTypes>, IInstruction<ISpecReferences>
    {
        public string Name => "LDS";

        public byte OPCode => 3;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;
            ISpecReferences references = (ISpecReferences)compiler;
            ISpecTypes types = (ISpecTypes)compiler;

            if (instruction.Parameters.Count != 3)
            {
                compiler.ThrowError(instruction, "LDS instruction must have 3 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            byte register, size;

            if (!registers.GetRegister(instruction.Parameters[0], out register))
            {
                compiler.ThrowError(instruction, "LDS instruction must have a register in the first parameter. The first parameter is not a register.");
                return new byte[] { 0 };
            }

            if (!types.GetTypeSize(instruction.Parameters[2], out size))
            {
                compiler.ThrowError(instruction, "LDS instruction must have a type in the third parameter. The third parameter is not a type or a size.");
                return new byte[] { 0 };
            }

            byte[] value = compiler.ParseValue(instruction.Parameters[1], ValueType.Integer);

            if (value == null)
            {
                if (!references.SetDefinitionBlock(instruction, instruction.Parameters[1], 2, 4))
                {
                    compiler.ThrowError(instruction, "LDS instruction must have a definition name or stack address.");
                    return new byte[] { OPCode, register, 0, 0, 0, 0, 0 };
                }

                value = new byte[] { 0, 0, 0, 0 };

            }
            else
            {
                value = compiler.FitDataInSize(instruction, value, 4, false);
            }

            List<byte> result = new List<byte>();
            result.Add(OPCode);
            result.Add(register);
            result.AddRange(value);
            result.Add(size);

            return result.ToArray();
        }
    }
}
