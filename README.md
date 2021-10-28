# MicroVASM .NET
A .NET implementation of the [MicroVASM](https://github.com/nevoa-dev/micro-vasm) assembly language.

# Installation and Usage
- Requires .NET Core 3.1

Build the console project and create a `main.vasm` on the output build folder.

# Example
This example will print numbers from 1 to 10. You must have a print function on your MicroVE VM.
```asm
FUNCTION print

SCOPE forLoop
    DEF     INT32 index 0
    LDI     R0 INT32 0
    LDI     R1 INT32 10
    LDI     R3 INT8 1
    REFERENCE   for_start
    CMP     GREATER_OR_EQUAL R2 R0 R1
    JMP     R2  for_end
    ; Body
    ADD     R0 R0 R3
    INVOKE  print
    JMP     R3 for_start
    STS     R0 index INT32
    REFERENCE for_end
END

EOP
```
