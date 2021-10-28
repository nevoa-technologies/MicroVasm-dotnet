using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionSTR : Instruction3Registers
    {
        public override string Name => "STR";

        public override byte OPCode => 2;
    }
}
