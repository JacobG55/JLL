using HarmonyLib;
using JLL.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JLL.Patches
{
    public static class JTranspilerHelper
    {
        public static IEnumerable<CodeInstruction> CodeEdit(IEnumerable<CodeInstruction> instructions, string debugName, CodeTest[] search, Action<List<CodeInstruction>> action)
            => CodeEdit(instructions, debugName, new(search, action));

        public static IEnumerable<CodeInstruction> CodeEdit(IEnumerable<CodeInstruction> instructions, string debugName, CodeOperation operation)
        {
            List<CodeInstruction> newInstructions = [];

            int found = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                newInstructions.Add(instruction);
                if (operation.Check(instruction, ref newInstructions))
                {
                    found++;
                }
            }

            JLogHelper.LogInfo($"Patched {debugName}: {found}", JLogLevel.Debuging);

            foreach (CodeInstruction instruction in newInstructions)
            {
                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> MultiCodeEdit(IEnumerable<CodeInstruction> instructions, string debugName, params CodeOperation[] operations)
        {
            List<CodeInstruction> newInstructions = [];

            int[] found = new int[operations.Length];
            for (int i = 0; i < found.Length; i++) found[i] = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                newInstructions.Add(instruction);
                for (int i = 0; i < operations.Length; i++)
                {
                    if (operations[i].Check(instruction, ref newInstructions))
                    {
                        found[i]++;
                        foreach (CodeOperation op in operations)
                        {
                            op.matches = 0;
                        }
                        break;
                    }
                }
            }

            JLogHelper.LogInfo($"Patched {debugName}: [{string.Join(", ", found)}]", JLogLevel.Debuging);

            foreach (CodeInstruction instruction in newInstructions)
            {
                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> AddAfter(IEnumerable<CodeInstruction> instructions, string debugName, CodeTest[] search, MethodInfo methodInfo, MethodParams methodParams = MethodParams.Empty, bool replace = false)
        {
            IEnumerable<CodeInstruction> paramInstructions = GetCodes(methodParams).Select((code) => new CodeInstruction(code));
            Action<List<CodeInstruction>> action =
            replace ? (list) =>
            {
                for (int i = 0; i < search.Length; i++) list.RemoveAt(list.Count - 1);
                list.AddRange(paramInstructions);
                list.Add(new(OpCodes.Call, methodInfo));
            } : (list) =>
            {
                list.AddRange(paramInstructions);
                list.Add(new(OpCodes.Call, methodInfo));
            };
            return CodeEdit(instructions, debugName, new(search, action));
        }

        public static IEnumerable<CodeInstruction> AddAfter(IEnumerable<CodeInstruction> instructions, string debugName, MethodInfo orig, MethodInfo methodInfo, MethodParams methodParams = MethodParams.Empty, bool replace = false)
        {
            List<CodeTest> tests = [];

            tests.AddRange(GetCodes(methodParams).Select((code) => new CodeTest(code)));
            tests.Add(new(OpCodes.Call, (code) => code.Calls(orig)));

            return AddAfter(instructions, debugName, tests.ToArray(), methodInfo, methodParams, replace);
        }

        public static IEnumerable<CodeInstruction> AddAfter(IEnumerable<CodeInstruction> instructions, string debugName, FieldInfo orig, MethodInfo methodInfo, MethodParams methodParams = MethodParams.Empty, bool replace = false)
        {
            List<CodeTest> tests = [];

            tests.AddRange(GetCodes(methodParams).Select((code) => new CodeTest(code)));
            tests.Add(new(OpCodes.Ldfld, (code) => code.LoadsField(orig)));

            return AddAfter(instructions, debugName, tests.ToArray(), methodInfo, methodParams, replace);
        }

        private static OpCode[] GetCodes(MethodParams methodParams)
        => methodParams switch
        {
            MethodParams.Self => [OpCodes.Ldarg_0],
            MethodParams.EnumerableSelf => [OpCodes.Ldloc_1],
            _ => []
        };
    }
    public struct CodeTest
    {
        public OpCode? OpCode;
        public Func<CodeInstruction, bool> Test;

        public CodeTest(OpCode code)
        {
            OpCode = code;
            Test = null;
        }

        public CodeTest(Func<CodeInstruction, bool> test)
        {
            OpCode = null;
            Test = test;
        }

        public CodeTest(OpCode code, Func<CodeInstruction, bool> test)
        {
            OpCode = code;
            Test = test;
        }

        public readonly bool Validate(CodeInstruction code)
        {
            if (OpCode != null && code.opcode != OpCode) return false;
            if (Test == null) return true;
            return Test.Invoke(code);
        }
    }

    public class CodeOperation(CodeTest[] test, Action<List<CodeInstruction>> action)
    {
        public CodeTest[] Test = test;
        public Action<List<CodeInstruction>> Action = action;
        public int matches = 0;

        public bool Check(CodeInstruction instruction, ref List<CodeInstruction> newInstructions)
        {
            if (Test[matches].Validate(instruction))
            {
                matches++;
                if (matches == Test.Length)
                {
                    Action.Invoke(newInstructions);
                    matches = 0;
                    return true;
                }
            }
            else matches = 0;
            return false;
        }
    }

    public enum MethodParams
    {
        Empty,
        Self,
        EnumerableSelf,
    }
}
