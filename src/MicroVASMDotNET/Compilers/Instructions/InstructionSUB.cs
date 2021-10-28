using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionSUB : Instruction3Registers
    {
        public override string Name => "SUB";

        public override byte OPCode => 10;
    }
}
