using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionLADR : IInstruction, IInstruction<ISpecRegisters>, IInstruction<ISpecTypes>, IInstruction<ISpecReferences>
    {
        public string Name => "LADR";

        public byte OPCode => 66;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;
            ISpecReferences references = (ISpecReferences)compiler;

            if (instruction.Parameters.Count != 2)
            {
                compiler.ThrowError(instruction, "LADR instruction must have 2 parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            byte register;
            int size;

            if (!registers.GetRegister(instruction.Parameters[0], out register))
            {
                compiler.ThrowError(instruction, "LADR instruction must have a register in the first parameter. The first parameter is not a register.");
                return new byte[] { 0 };
            }


           if (!references.SetDefinitionBlock(instruction, instruction.Parameters[1], 2, 4))
           {
               compiler.ThrowError(instruction, "LADR instruction must have a definition name or stack address.");
               return new byte[] { OPCode, register, 0, 0, 0, 0, 0 };
           }

           byte[] adr = new byte[] { 0, 0, 0, 0 };


            List<byte> result = new List<byte>();
            result.Add(OPCode);
            result.Add(register);
            result.AddRange(adr);

            return result.ToArray();
        }
    }
}
