namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecRegisters : ICompilerSpec
    {
        bool GetRegister(string name, out byte register);
    }
}
