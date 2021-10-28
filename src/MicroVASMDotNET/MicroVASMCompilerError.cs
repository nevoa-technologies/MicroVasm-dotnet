namespace MicroVASMDotNET
{
    public class MicroVASMCompilerError
    {
        public VASMInstructionData Instruction { get; }
        public string Message { get; }


        internal MicroVASMCompilerError(VASMInstructionData instruction, string message)
        {
            this.Instruction = instruction;
            this.Message = message;
        }
    }
}
