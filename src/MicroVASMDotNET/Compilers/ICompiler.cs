using System;

namespace MicroVASMDotNET.Compilers
{
    public interface ICompiler
    {
        byte[] Generate();
        void ThrowError(VASMInstructionData instruction, string message);
        void ThrowWarning(VASMInstructionData instruction, string message);
        UInt32 CurrentBytecodeLength { get; }
        byte[] ParseValue(string s, ValueType type);
        byte[] FitDataInSize(VASMInstructionData instruction, byte[] data, int size, bool showWarnings = true);
    }


    public enum ValueType
    {
        Integer,
        Float,
        Double
    }
}
