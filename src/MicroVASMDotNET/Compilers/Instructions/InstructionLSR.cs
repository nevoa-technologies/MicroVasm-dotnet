using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionLSR : Instruction3Registers
    {
        public override string Name => "LSR";

        public override byte OPCode => 23;
    }
}
