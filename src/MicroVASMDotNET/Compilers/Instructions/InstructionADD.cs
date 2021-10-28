using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers.Instructions
{
    public class InstructionADD : Instruction3Registers
    {
        public override string Name => "ADD";

        public override byte OPCode => 9;
    }
}
