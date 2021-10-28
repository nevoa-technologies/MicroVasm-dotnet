using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionMUL : Instruction3Registers
    {
        public override string Name => "MUL";

        public override byte OPCode => 11;
    }
}
