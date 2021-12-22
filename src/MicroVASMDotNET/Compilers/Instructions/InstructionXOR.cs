using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionXOR : Instruction3Registers
    {
        public override string Name => "XOR";

        public override byte OPCode => 24;
    }
}
