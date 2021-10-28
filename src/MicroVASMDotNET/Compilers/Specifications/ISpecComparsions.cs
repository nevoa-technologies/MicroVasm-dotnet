namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecComparsions : ICompilerSpec
    {
        bool GetComparsionCode(string name, out byte comparsion);
    }
}
