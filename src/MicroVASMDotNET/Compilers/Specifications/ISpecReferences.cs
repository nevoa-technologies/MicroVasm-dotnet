using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Specifications
{
    public interface ISpecReferences : ICompilerSpec
    {
        bool SetReferenceBlock(VASMInstructionData instructionData, string name, int offset, int size);
        bool SetDefinitionBlock(VASMInstructionData instructionData, string name, int offset, int size);
    }
}
