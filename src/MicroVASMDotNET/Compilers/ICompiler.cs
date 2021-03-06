using System;

namespace MicroVASMDotNET.Compilers
{
    public interface ICompiler
    {
        byte MaxTypeSize { get; }
        CompilerResult Generate();
        void ThrowError(VASMInstructionData instruction, string message);
        void ThrowWarning(VASMInstructionData instruction, string message);
        UInt32 CurrentBytecodeLength { get; }
        byte[] ParseValue(string s, ValueType type, out bool isNegativeInt);
        byte[] FitDataInSize(VASMInstructionData instruction, byte[] data, bool isNegativeInt, bool isUnsigned, int size, bool showWarnings = true);
    }


    public enum ValueType
    {
        Integer
    }
}
