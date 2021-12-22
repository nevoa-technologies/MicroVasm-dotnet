using MicroVASMDotNET.Compilers.Instructions;
using MicroVASMDotNET.Compilers.PreProcessing;
using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroVASMDotNET.Compilers
{
    public class MicroVASMCompiler_1_0 : MicroVASMBaseCompiler, ISpecRegisters, ISpecTypes, ISpecReferences, ISpecComparsions
    {
        protected Dictionary<string, byte> TypeSizes { get; set; }
        protected Dictionary<string, ValueType> TypeValueTypes { get; set; }
        protected Dictionary<string, byte> ComparsionCodes { get; set; }


        private UInt32 currentBytecodeLength = 0;

        public override UInt32 CurrentBytecodeLength => currentBytecodeLength;

        private PreProcessorFunction functionPreProcessor = new PreProcessorFunction();
        private PreProcessorDefinitions definitionsPreProcessor = new PreProcessorDefinitions();

        private List<byte[]> generatedInstructions = new List<byte[]>();

        private List<PostProcessData> postProcessReferences = new List<PostProcessData>();
        private List<PostDefinitionData> postProcessDefinitions = new List<PostDefinitionData>();


        public MicroVASMCompiler_1_0()
        {
            DeclareInstruction<InstructionEOP>();
            DeclareInstruction<InstructionLDR>();
            DeclareInstruction<InstructionSTR>();
            DeclareInstruction<InstructionLDS>();
            DeclareInstruction<InstructionSTS>();
            DeclareInstruction<InstructionLDI>();
            DeclareInstruction<InstructionMOV>();
            DeclareInstruction<InstructionNEG>();
            /* INVOKE is handled on PreProcessorFunction. */
            DeclareInstruction<InstructionADD>();
            DeclareInstruction<InstructionSUB>();
            DeclareInstruction<InstructionMUL>();
            DeclareInstruction<InstructionDIV>();
            /* SCOPE is handled on PreProcessorDefinitions. */
            /* END is handled on PreProcessorDefinitions. */
            DeclareInstruction<InstructionCMP>();
            DeclareInstruction<InstructionJMP>();
            DeclareInstruction<InstructionJNZ>();
            DeclareInstruction<InstructionCALL>();
            DeclareInstruction<InstructionAND>();
            DeclareInstruction<InstructionNOT>();
            DeclareInstruction<InstructionORR>();
            DeclareInstruction<InstructionLSL>();
            DeclareInstruction<InstructionLSR>();
            DeclareInstruction<InstructionXOR>();

            InitVariableTypes();
            InitComparsionCodes();
        }


        protected virtual void InitVariableTypes()
        {
            TypeSizes = new Dictionary<string, byte>();

            TypeSizes.Add("INT8", 1);
            TypeSizes.Add("INT16", 2);
            TypeSizes.Add("INT32", 4);
            TypeSizes.Add("INT64", 8);
            TypeSizes.Add("UINT8", 1);
            TypeSizes.Add("UINT16", 2);
            TypeSizes.Add("UINT32", 4);
            TypeSizes.Add("UINT64", 8);
            TypeSizes.Add("FLOAT", 4);
            TypeSizes.Add("DOUBLE", 8);

            TypeValueTypes = new Dictionary<string, ValueType>();

            TypeValueTypes.Add("INT8", ValueType.Integer);
            TypeValueTypes.Add("INT16", ValueType.Integer);
            TypeValueTypes.Add("INT32", ValueType.Integer);
            TypeValueTypes.Add("INT64", ValueType.Integer);
            TypeValueTypes.Add("UINT8", ValueType.Integer);
            TypeValueTypes.Add("UINT16", ValueType.Integer);
            TypeValueTypes.Add("UINT32", ValueType.Integer);
            TypeValueTypes.Add("UINT64", ValueType.Integer);
            TypeValueTypes.Add("FLOAT", ValueType.Float);
            TypeValueTypes.Add("DOUBLE", ValueType.Double);
        }


        protected virtual void InitComparsionCodes()
        {
            ComparsionCodes = new Dictionary<string, byte>();

            ComparsionCodes.Add("EQUAL", 0);
            ComparsionCodes.Add("NOT_EQUAL", 1);
            ComparsionCodes.Add("GREATER", 2);
            ComparsionCodes.Add("LESS", 3);
            ComparsionCodes.Add("GREATER_OR_EQUAL", 4);
            ComparsionCodes.Add("LESS_OR_EQUAL", 5);
        }


        public virtual bool GetRegister(string name, out byte register)
        {
            if (name.StartsWith("R"))
            {
                name = name.Remove(0, 1);

                if (Byte.TryParse(name, out register))
                {
                    return true;
                }
            }
            else if (name.Equals("SP"))
            {
                register = 5;
                return true;
            }

            register = 0;
            return false;
        }
        

        public bool GetTypeSize(string name, out byte size)
        {
            if (TypeSizes.ContainsKey(name))
            {
                size = TypeSizes[name];
                return true;
            }

            size = 0;

            if (byte.TryParse(name, out byte i))
            {
                size = i;
                return true;
            }

            return false;
        }


        public bool GetTypeValue(string name, out ValueType type)
        {
            if (TypeValueTypes.ContainsKey(name))
            {
                type = TypeValueTypes[name];
                return true;
            }

            type = ValueType.Integer;

            if (int.TryParse(name, out int result))
                return true;

            return false;
        }


        public bool GetComparsionCode(string name, out byte comparsion)
        {
            if (ComparsionCodes.ContainsKey(name))
            {
                comparsion = ComparsionCodes[name];
                return true;
            }

            comparsion = 0;
            return false;
        }


        public override byte[] Generate()
        {
            List<byte> result = new List<byte>();
            
            List<KeyValuePair<long, PostProcessData>> references = new List<KeyValuePair<long, PostProcessData>>();
            List<KeyValuePair<long, PostDefinitionData>> definitions = new List<KeyValuePair<long, PostDefinitionData>>();

            for (int i = 0; i < generatedInstructions.Count; i++)
            {
                byte[] bytes = generatedInstructions[i];

                foreach (var reference in postProcessReferences)
                {
                    if (reference.InstructionIndex == i)
                    {
                        references.Add(new KeyValuePair<long, PostProcessData>(result.Count + reference.Offset, reference));
                    }
                }

                foreach (var definition in postProcessDefinitions)
                {
                    if (definition.InstructionIndex == i)
                    {
                        definitions.Add(new KeyValuePair<long, PostDefinitionData>(result.Count + definition.Offset, definition));
                    }
                }

                result.AddRange(bytes);
            }

            foreach (var r in references)
            {
                if (definitionsPreProcessor.GetReferenceIndex((long) r.Value.Data, out UInt32 index))
                {
                    byte[] bytes = BitConverter.GetBytes(index).AsLittleEndian();

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        result[i + (int) r.Key] = bytes[i];
                    }
                }
                else
                {
                    throw new Exception("Reference not found.");
                }
            }

            foreach (var d in definitions)
            {
                if (definitionsPreProcessor.GetDefinitionIndex(d.Value.Line, d.Value.Name, out Int32 index))
                {
                    byte[] bytes = BitConverter.GetBytes(index).AsLittleEndian();

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        result[i + (int)d.Key] = bytes[i];
                    }
                }
                else
                {
                    throw new Exception("Definition not found.");
                }
            }

            result.Add(0);

            return result.ToArray();
        }


        protected override void StartGenerating()
        {
            UInt16 versionMajor = 1;
            UInt16 versionMinor = 0;

            byte[] major = BitConverter.GetBytes(versionMajor).AsLittleEndian();
            byte[] minor = BitConverter.GetBytes(versionMinor).AsLittleEndian();

            string[] functionNames = functionPreProcessor.BuildFunctionNamesArray();
            byte[] functionsCount = BitConverter.GetBytes((UInt32) functionNames.Length);

            List<byte> result = new List<byte>();
            result.AddRange(major);
            result.AddRange(minor);
            result.AddRange(functionsCount);
            result.AddRange(Encoding.ASCII.GetBytes(String.Join("\0", functionNames) + '\0'));
            result.AddRange(definitionsPreProcessor.GetScopeMemory(0));

            currentBytecodeLength = (UInt32) result.Count;
            generatedInstructions.Add(result.ToArray());

            definitionsPreProcessor.StartProcessing(this);
            functionPreProcessor.StartProcessing(this);
        }


        protected override void ProcessInstruction(VASMInstructionData instruction, bool IsPreProcessing)
        {
            if (IsPreProcessing)
            {
                functionPreProcessor.PreProcessInstruction(this, instruction);
                definitionsPreProcessor.PreProcessInstruction(this, instruction);
                return;
            }
            else
            {
                List<byte> instructionBytes = new List<byte>();

                if (AvailableInstructions.ContainsKey(instruction.Instruction))
                {
                    instructionBytes.AddRange(AvailableInstructions[instruction.Instruction].Generate(this, instruction));
                }
                else
                {
                    byte[] functionBytes = functionPreProcessor.ProcessInstruction(this, instruction);
                    byte[] defintionBytes = definitionsPreProcessor.ProcessInstruction(this, instruction);


                    if (functionBytes == null && defintionBytes == null)
                        ThrowError(instruction, "Unrecognized instruction: " + instruction.Instruction);

                    instructionBytes.AddRange(functionBytes != null ? functionBytes : new byte[0]);
                    instructionBytes.AddRange(defintionBytes != null ? defintionBytes : new byte[0]);
                }

                currentBytecodeLength += (UInt32) instructionBytes.Count;

                generatedInstructions.Add(instructionBytes.ToArray());
            }
        }


        public bool SetReferenceBlock(VASMInstructionData instructionData, string name, int offset, int size)
        {
            object data;

            if (!definitionsPreProcessor.HasReference(instructionData.LineAsIndex, name, out data))
                return false;

            postProcessReferences.Add(new PostProcessData(generatedInstructions.Count, offset, size, data));

            return true;
        }


        public bool SetDefinitionBlock(VASMInstructionData instructionData, string name, int offset, int size)
        {
            if (!definitionsPreProcessor.HasDefinition(instructionData.LineAsIndex, name))
                return false;

            postProcessDefinitions.Add(new PostDefinitionData(generatedInstructions.Count, instructionData.LineAsIndex, name, offset));

            return true;
        }


        private class PostProcessData
        {
            public int InstructionIndex { get; set; }
            public int Offset { get; set; }
            public int Size { get; set; }
            public object Data { get; set; }

            public PostProcessData(int instructionIndex, int offset, int size, object data)
            {
                InstructionIndex = instructionIndex;
                Offset = offset;
                Size = size;
                this.Data = data;
            }
        }


        private class PostDefinitionData
        {
            public int InstructionIndex { get; set; }
            public int Line { get; set; }
            public string Name { get; set; }
            public int Offset { get; set; }

            public PostDefinitionData(int instructionIndex, int line, string name, int offset)
            {
                this.InstructionIndex = instructionIndex;
                this.Line = line;
                this.Name = name;
                this.Offset = offset;
            }
        }
    }
}
