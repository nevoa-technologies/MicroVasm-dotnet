using System;
using System.Collections.Generic;

namespace MicroVASMDotNET
{
    public class VASMCodeData : IDisposable
    {
        public List<VASMInstructionData> instructions { get; }


        internal VASMCodeData(List<VASMInstructionData> instructions)
        {
            this.instructions = instructions;
        }


        public void Dispose()
        {
            instructions.Clear();
        }
    }
}
