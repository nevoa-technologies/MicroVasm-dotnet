using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionLDI : IInstruction, IInstruction<ISpecRegisters>, IInstruction<ISpecTypes>, IInstruction<ISpecReferences>
    {
        public string Name => "LDI";

        public byte OPCode => 5;

        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;
            ISpecTypes types = (ISpecTypes)compiler;
            ISpecReferences references = (ISpecReferences)compiler;

            if (instruction.Parameters.Count != 3)
            {
                compiler.ThrowError(instruction, "LDI instruction must have 3 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            if (registers.GetRegister(instruction.Parameters[0], out byte register))
            {
                if (types.GetTypeSize(instruction.Parameters[1], out int size) && types.GetTypeValue(instruction.Parameters[1], out ValueType type))
                {
                    byte[] bytes = compiler.ParseValue(instruction.Parameters[2], type, out bool isNegativeInt);

                    if (size > compiler.MaxTypeSize)
                    {
                        compiler.ThrowError(instruction, "LDI instruction must have a type with a size not bigger than " + compiler.MaxTypeSize + ".");
                        return new byte[] { 0 };
                    }

                    if (bytes != null)
                    {
                        bytes = compiler.FitDataInSize(instruction, bytes, isNegativeInt, size);

                        List<byte> result = new List<byte>();
                        result.Add(OPCode);
                        result.Add(register);
                        result.Add((byte) size);

                        result.AddRange(bytes);
                        return result.ToArray();
                    }


                    if (references.SetDefinitionBlock(instruction, instruction.Parameters[2], 3, 4))
                    {
                        List<byte> result = new List<byte>();
                        result.Add(OPCode);
                        result.Add(register);
                        result.Add((byte)4);
                        result.AddRange(new byte[4]);
                        return result.ToArray();
                    }

                    compiler.ThrowError(instruction, "LDI must have a valid value.");
                    return new byte[] { 0 };
                }

                compiler.ThrowError(instruction, "LDI instruction must have a second parameter as a type. The second parameter is not a type.");
                return new byte[] { 0 };
            }

            compiler.ThrowError(instruction, "LDI instruction must have a first parameter as a register. The first parameter is not a register.");
            return new byte[] { 0 };

        }
    }
}
