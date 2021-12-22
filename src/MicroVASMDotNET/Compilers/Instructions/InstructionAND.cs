using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionAND : Instruction3Registers
    {
        public override string Name => "AND";

        public override byte OPCode => 19;
    }
}
