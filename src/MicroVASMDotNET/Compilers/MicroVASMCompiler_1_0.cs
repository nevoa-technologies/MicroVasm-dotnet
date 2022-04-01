using MicroVASMDotNET.Compilers.Instructions;
using MicroVASMDotNET.Compilers.PreProcessing;
using MicroVASMDotNET.Compilers.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MicroVASMDotNET.Compilers.PreProcessing.PreProcessorDefinitions;

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
        private string[] functionNames;

        private List<PostProcessData> postProcessReferences = new List<PostProcessData>();
        private List<PostDefinitionData> postProcessDefinitions = new List<PostDefinitionData>();

        private byte minRegisters = 7;


        public MicroVASMCompiler_1_0(bool is64Bit) : base(is64Bit)
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
            DeclareInstruction<InstructionINC>();
            DeclareInstruction<InstructionDEC>();

            DeclareInstruction<InstructionPUSH>();
            DeclareInstruction<InstructionPOP>();
            DeclareInstruction<InstructionLADR>();
            
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

            TypeValueTypes = new Dictionary<string, ValueType>();

            TypeValueTypes.Add("INT8", ValueType.Integer);
            TypeValueTypes.Add("INT16", ValueType.Integer);
            TypeValueTypes.Add("INT32", ValueType.Integer);
            TypeValueTypes.Add("INT64", ValueType.Integer);
            TypeValueTypes.Add("UINT8", ValueType.Integer);
            TypeValueTypes.Add("UINT16", ValueType.Integer);
            TypeValueTypes.Add("UINT32", ValueType.Integer);
            TypeValueTypes.Add("UINT64", ValueType.Integer);
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
                    if (register + 1 > minRegisters)
                        minRegisters = (byte) (register + 1);

                    return true;
                }
            }
            else if (name.Equals("SP"))
            {
                register = 5;
                return true;
            }
            else if (name.Equals("MP"))
            {
                register = 6;
                return true;
            }

            register = 0;
            return false;
        }
        

        public bool GetTypeSize(string name, out int size)
        {
            if (TypeSizes.ContainsKey(name))
            {
                size = TypeSizes[name];
                return true;
            }

            size = 0;

            if (int.TryParse(name, out int i))
            {
                size = i;
                return true;
            }

            return false;
        }


        public bool GetTypeSizeDef(string name, out uint size)
        {
            if (TypeSizes.ContainsKey(name))
            {
                size = TypeSizes[name];
                return true;
            }

            size = 0;

            if (uint.TryParse(name, out uint i))
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


        public override CompilerResult Generate()
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

            CompilerResult compilerResult = new CompilerResult();
            compilerResult.Result = result.ToArray();
            compilerResult.ExternalFunctionsCount = functionNames.Length;


            // Calculate the size of the memory used by function names.
            // We add +1 because of the \0 at the end of each name.
            int functionNamesLength = functionNames.Sum(f => f.Length + 1);

            compilerResult.MemoryFunctionNamesUsage = functionNamesLength;

            var scopeCallTrees = definitionsPreProcessor.BuildScopeCallTrees();
            var rootScope = scopeCallTrees.Keys.ElementAt(0);

            int minScopesCount = 1;
            int stackMemoryUsage = GetCallMemorySize(scopeCallTrees, rootScope, 0, ref minScopesCount);
            compilerResult.MinStackUsage = stackMemoryUsage;
            compilerResult.MinScopesCount = minScopesCount;

            compilerResult.MemoryDynamicUsage = definitionsPreProcessor.GetDynamicMemoryUsage(out bool freedAll);

            if (!freedAll)
            {
                ThrowError(null, "There is memory that is not being freed correctly.");
            }

            compilerResult.HasErrors = CompileErrors.Count > 0;
            compilerResult.MinRegisters = minRegisters;

            return compilerResult;
        }


        int GetCallMemorySize(Dictionary<ScopeInfo, Stack<ScopeInfo>> scopeCallTree, ScopeInfo scope, int currentSize, ref int scopeStackCount)
        {
            int currentMaxSize = 0;
            int maxStackCount = 0;

            var stack = scopeCallTree[scope];

            // Check if it this function is being called by one of its childs.
            HashSet<ScopeInfo> childScopes = new HashSet<ScopeInfo>();
            Stack<ScopeInfo> scopesToCheck = new Stack<ScopeInfo>();

            foreach (var scopeCall in stack)
            {
                childScopes.Add(scopeCall);
                scopesToCheck.Push(scopeCall);
            }

            while (scopesToCheck.Count > 0)
            {
                ScopeInfo s = scopesToCheck.Pop();

                foreach (var scopeCall in scopeCallTree[s])
                {
                    if (childScopes.Contains(scopeCall))
                        break;

                    childScopes.Add(scopeCall);
                    scopesToCheck.Push(scopeCall);
                }
            }

            if (childScopes.Contains(scope))
            {
                ThrowError(null, "You cannot call scopes recursively. '" + scope.Name + "' is being called by itself or other scope recursively.");
                scopeStackCount = scopeStackCount + maxStackCount;
                return 0;
            }


            foreach (var scopeCall in stack)
            {
                int stackCount = 1;
                int callSize = GetCallMemorySize(scopeCallTree, scopeCall, currentSize, ref stackCount);

                if (callSize > currentMaxSize)
                    currentMaxSize = callSize;

                if (stackCount > maxStackCount)
                    maxStackCount = stackCount;
            }

            scopeStackCount = scopeStackCount + maxStackCount;


            return currentSize + currentMaxSize + scope.GetDefinitionsDataSize();
        }


        protected override void StartGenerating()
        {
            UInt16 versionMajor = 1;
            UInt16 versionMinor = 0;

            byte[] major = BitConverter.GetBytes(versionMajor).AsLittleEndian();
            byte[] minor = BitConverter.GetBytes(versionMinor).AsLittleEndian();

            functionNames = functionPreProcessor.BuildFunctionNamesArray();
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
