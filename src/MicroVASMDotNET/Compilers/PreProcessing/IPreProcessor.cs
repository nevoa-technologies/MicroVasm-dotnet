using System.Collections.Generic;

namespace MicroVASMDotNET.Compilers.PreProcessing
{
    public interface IPreProcessor
    {  
        void PreProcessInstruction(ICompiler compiler, VASMInstructionData instruction);
        void StartProcessing(ICompiler compiler);
        byte[] ProcessInstruction(ICompiler compiler, VASMInstructionData instruction);
    }


    public interface IPreProcessor<T> where T : ICompilerSpec { }
}
