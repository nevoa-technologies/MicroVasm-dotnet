﻿using MicroVASMDotNET.Compilers.Instructions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace MicroVASMDotNET.Compilers
{
    public abstract class MicroVASMBaseCompiler : ICompiler
    {
        protected Dictionary<string, IInstruction> AvailableInstructions { get; } = new Dictionary<string, IInstruction>();


        private bool isPreProcessing;

        public abstract UInt32 CurrentBytecodeLength { get; }


        private List<MicroVASMCompilerError> CompileErrors { get; } = new List<MicroVASMCompilerError>();


        protected void DeclareInstruction<T>() where T : IInstruction, new()
        {
            var interfaces = typeof(T).GetInterfaces();
            T newInstruction = new T();

            Type[] thisInterfaces = GetType().GetInterfaces();

            foreach (var interf in interfaces)
            {
                if (interf.IsGenericType && interf.GetGenericTypeDefinition() == typeof(IInstruction<>) && interf.GenericTypeArguments.Length > 0)
                {
                    if (thisInterfaces.Contains(interf.GenericTypeArguments[0]))
                    {
                        AvailableInstructions.Add(newInstruction.Name, newInstruction);
                        return;
                    }
                }
            }

            if (interfaces.Length == 1)
            {
                AvailableInstructions.Add(newInstruction.Name, newInstruction);
                return;
            }
            else
                throw new Exception("Invalid instruction. The instruction: \"" + newInstruction.Name + "\" of type " + typeof(T).Name + " is not compatible with the compiler " + GetType().Name + ".");
        }


        public void StartPreProcessing()
        {
            isPreProcessing = true;
        }


        public void StopPreProcessing()
        {
            isPreProcessing = false;
            StartGenerating();
        }


        public void NextInstruction(VASMInstructionData instruction)
        {
            ProcessInstruction(instruction, isPreProcessing);
        }


        protected abstract void StartGenerating();
        protected abstract void ProcessInstruction(VASMInstructionData instruction, bool IsPreProcessing);


        public abstract byte[] Generate();


        public void ThrowError(VASMInstructionData instruction, string message)
        {
            CompileErrors.Add(new MicroVASMCompilerError(instruction, message));
            Console.WriteLine("Error: " + message + " Line: " + instruction.RealLine);
        }

        public void ThrowWarning(VASMInstructionData instruction, string message)
        {
            Console.WriteLine("Warning: " + message + " Line: " + instruction.RealLine);
        }


        public byte[] ParseValue(string s, ValueType type, out bool isNegativeInt)
        {
            isNegativeInt = false;

            byte[] bytes;

            if (s.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                bytes = BitConverter.GetBytes(long.Parse(s.Substring(2), NumberStyles.HexNumber)).AsLittleEndian();
            else if (s.StartsWith("0o", StringComparison.InvariantCultureIgnoreCase))
                bytes = BitConverter.GetBytes((long)Convert.ToInt64(s.Substring(2), 8)).AsLittleEndian();
            else if (s.StartsWith("0b", StringComparison.InvariantCultureIgnoreCase))
                bytes = BitConverter.GetBytes((long)Convert.ToInt64(s.Substring(2), 2)).AsLittleEndian();
            else
            {
                if (type == ValueType.Float)
                {
                    if (float.TryParse(s, out float result))
                        bytes = BitConverter.GetBytes((float)Convert.ToDouble(result)).AsLittleEndian();
                    else
                        return null;
                }
                else if (type == ValueType.Double)
                {
                    if (double.TryParse(s, out double result))
                        bytes = BitConverter.GetBytes(Convert.ToDouble(result)).AsLittleEndian();
                    else
                        return null;
                }
                else
                {
                    if (s.StartsWith("-"))
                    {
                        isNegativeInt = true;
                    }

                    if (long.TryParse(s, out long result))
                        bytes = BitConverter.GetBytes(result).AsLittleEndian();
                    else
                        return null;
                }
            }

            return bytes;
        }


        public byte[] FitDataInSize(VASMInstructionData instruction, byte[] data, bool isNegativeInt, int size, bool showWarnings = true)
        {
            List<byte> finalBytes = new List<byte>(data);

            while (finalBytes.Count < size)
                if (isNegativeInt)
                    finalBytes.Add(0xFF);
                else
                    finalBytes.Add(0x00);

            for (int i = size; i < finalBytes.Count; i++)
            {
                if ((isNegativeInt && finalBytes[i] != 0xFF) || (!isNegativeInt && finalBytes[i] != 0))
                {
                    if (showWarnings)
                        ThrowWarning(instruction, "Value is bigger than its storage size.");
                    break;
                }
            }

            if (finalBytes.Count > size)
                finalBytes.RemoveRange(size, finalBytes.Count - size);

            return finalBytes.ToArray();
        }
    }
}
