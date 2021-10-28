using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionLDR : Instruction3Registers
    {
        public override string Name => "LDR";

        public override byte OPCode => 1;
    }
}
