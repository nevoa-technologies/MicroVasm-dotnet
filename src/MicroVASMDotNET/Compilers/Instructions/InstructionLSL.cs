using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionLSL : Instruction3Registers
    {
        public override string Name => "LSL";

        public override byte OPCode => 21;
    }
}
