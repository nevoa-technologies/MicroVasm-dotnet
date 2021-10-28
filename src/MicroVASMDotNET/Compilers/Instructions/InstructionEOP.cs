namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionEOP : IInstruction
    {
        public string Name => "EOP";

        public byte OPCode => 0;

        public byte[] Generate(ICompiler compiler, VASMInstructionData instruction)
        {
            if (instruction.HasParameters)
                compiler.ThrowError(instruction, "EOP instruction cannot have parameters. " + instruction.Parameters.Count + " parameters found.");

            return new byte[] { OPCode };
        }
    }
}
