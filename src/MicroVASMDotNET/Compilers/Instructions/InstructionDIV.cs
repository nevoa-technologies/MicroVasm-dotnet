using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionDIV : Instruction3Registers
    {
        public override string Name => "DIV";

        public override byte OPCode => 12;
    }
}
