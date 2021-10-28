namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecTypes : ICompilerSpec
    {
        bool GetTypeSize(string name, out byte size);
        bool GetTypeValue(string name, out ValueType type);
    }
}
