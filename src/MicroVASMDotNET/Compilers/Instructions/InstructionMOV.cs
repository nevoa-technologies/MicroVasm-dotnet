using MicroVASMDotNET.Compilers.Specifications;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionMOV : IInstruction, IInstruction<ISpecRegisters>
    {
        public string Name => "MOV";

        public byte OPCode => 6;


        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            ISpecRegisters registers = (ISpecRegisters)compiler;

            if (instruction.Parameters.Count != 2)
            {
                compiler.ThrowError(instruction, "MOV instruction must have 2 register parameters. " + instruction.Parameters.Count + " parameters found.");
                return new byte[] { 0 };
            }

            byte register1, register2;

            if (!registers.GetRegister(instruction.Parameters[0], out register1))
            {
                compiler.ThrowError(instruction, "MOV instruction must have 2 register parameters. The first parameter is not a register.");
                return new byte[] { 0 };
            }

            if (!registers.GetRegister(instruction.Parameters[1], out register2))
            {
                compiler.ThrowError(instruction, "MOV instruction must have 2 register parameters. The second parameter is not a register.");
                return new byte[] { 0 };
            }

            return new byte[] { OPCode, register1, register2 };
        }
    }
}
