using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET
{
    public class CompilerResult
    {
        public byte[] Result { get; internal set; }
        public int ExternalFunctionsCount { get; internal set; }
        public int MinScopesCount { get; internal set; }
        public int MemoryFunctionNamesUsage { get; internal set; }
        public int MemoryDynamicUsage { get; internal set; }
        public int MinStackUsage { get; internal set; }
        public bool HasErrors { get; internal set; }
        public int MinRegisters { get; internal set; }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("External Functions Count:          " + ExternalFunctionsCount);
            builder.AppendLine("Scope Stack Count:                 " + MinScopesCount);
            builder.AppendLine("Memory Usage of Function Names:    " + MemoryFunctionNamesUsage);
            builder.AppendLine("Memory Usage of Dynamic Stack:     " + MemoryDynamicUsage);
            builder.AppendLine("Data Stack Usage:                  " + MinStackUsage);
            builder.AppendLine("Highest Register:                  " + (MinRegisters - 1));
            return builder.ToString();
        }
    }
}
