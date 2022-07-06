namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecTypes : ICompilerSpec
    {
        bool GetTypeSize(string name, out int size, out bool isUnsigned);
        bool GetTypeValue(string name, out ValueType type);
    }
}
