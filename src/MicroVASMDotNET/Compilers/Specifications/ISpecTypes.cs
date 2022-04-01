namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecTypes : ICompilerSpec
    {
        bool GetTypeSize(string name, out int size);
        bool GetTypeValue(string name, out ValueType type);
    }
}
