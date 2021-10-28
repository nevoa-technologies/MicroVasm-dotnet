using System.Collections.Generic;

namespace MicroVASMDotNET
{
    public class VASMInstructionData
    {
        public string Instruction { get; }
        public List<string> Parameters { get; }
        public int LineAsIndex { get; }
        public int RealLine => LineAsIndex + 1;

        public bool HasParameters => Parameters.Count > 0;


        internal VASMInstructionData(string instruction, List<string> parameters, int line)
        {
            this.Instruction = instruction;
            this.Parameters = parameters;
            this.LineAsIndex = line;
        }
    }
}
