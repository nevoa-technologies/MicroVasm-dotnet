namespace MicroVASMDotNET.Compilers.Instructions
{
    public interface IInstruction
    {
        string Name { get; }
        byte OPCode { get; }
        byte[] Generate(ICompiler compiler, VASMInstructionData instruction);
    }


    public interface IInstruction<T> where T : ICompilerSpec { }
}
