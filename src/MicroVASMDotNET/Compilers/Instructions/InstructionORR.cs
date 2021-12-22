using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionORR : Instruction3Registers
    {
        public override string Name => "ORR";

        public override byte OPCode => 20;
    }
}
