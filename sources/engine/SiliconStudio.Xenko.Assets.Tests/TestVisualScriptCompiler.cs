﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using SiliconStudio.Core.Diagnostics;
using SiliconStudio.Xenko.Assets.Scripts;

namespace SiliconStudio.Xenko.Assets.Tests
{
    [TestFixture]
    public class TestVisualScriptCompiler
    {
        [Test]
        public void TestCustomCode()
        {
            var visualScript = new VisualScriptAsset();

            // Build blocks
            var functionStart = new FunctionStartBlock();
            var writeTrue = new CustomCodeBlock { Code = "System.Console.Write(true);" };
            var method = new Method();
            method.Blocks.Add(functionStart);
            method.Blocks.Add(writeTrue);

            // Generate slots
            foreach (var block in method.Blocks)
                block.GenerateSlots(block.Slots, new SlotGeneratorContext());

            // Build links
            method.Links.Add(new Link(functionStart, writeTrue));

            visualScript.Methods.Add(method);

            // Test
            TestAndCompareOutput(visualScript, "True", testInstance => testInstance.Test());
        }

        [Test]
        public void TestConditionalExpression()
        {
            var visualScript = new VisualScriptAsset();

            // Build blocks
            var functionStart = new FunctionStartBlock();
            var conditionalBranch = new ConditionalBranchBlock();
            var writeTrue = new CustomCodeBlock { Code = "System.Console.Write(true);" };
            var writeFalse = new CustomCodeBlock { Code = "System.Console.Write(false);" };
            var method = new Method();
            method.Blocks.Add(functionStart);
            method.Blocks.Add(conditionalBranch);
            method.Blocks.Add(writeTrue);
            method.Blocks.Add(writeFalse);

            // Generate slots
            foreach (var block in method.Blocks)
                block.GenerateSlots(block.Slots, new SlotGeneratorContext());

            // Build links
            method.Links.Add(new Link(functionStart, conditionalBranch));
            method.Links.Add(new Link(conditionalBranch.TrueSlot, writeTrue));
            method.Links.Add(new Link(conditionalBranch.FalseSlot, writeFalse));

            visualScript.Methods.Add(method);

            // Test
            conditionalBranch.ConditionSlot.Value = "true";
            TestAndCompareOutput(visualScript, "True", testInstance => testInstance.Test());

            conditionalBranch.ConditionSlot.Value = "false";
            TestAndCompareOutput(visualScript, "False", testInstance => testInstance.Test());
        }

        [Test]
        public void TestVariableGet()
        {
            var visualScript = new VisualScriptAsset();

            var condition = new Property("bool", "Condition");
            visualScript.Properties.Add(condition);

            // Build blocks
            // TODO: Switch to a simple Write(variable) later, so that we don't depend on ConditionalBranchBlock for this test?
            var functionStart = new FunctionStartBlock();
            var conditionGet = new VariableGet { Name = condition.Name };
            var conditionalBranch = new ConditionalBranchBlock();
            var writeTrue = new CustomCodeBlock { Code = "System.Console.Write(true);" };
            var writeFalse = new CustomCodeBlock { Code = "System.Console.Write(false);" };
            var method = new Method();
            method.Blocks.Add(functionStart);
            method.Blocks.Add(conditionGet);
            method.Blocks.Add(conditionalBranch);
            method.Blocks.Add(writeTrue);
            method.Blocks.Add(writeFalse);

            // Generate slots
            foreach (var block in method.Blocks)
                block.GenerateSlots(block.Slots, new SlotGeneratorContext());

            // Build links
            method.Links.Add(new Link(functionStart, conditionalBranch));
            method.Links.Add(new Link(conditionGet.ValueSlot, conditionalBranch.ConditionSlot));
            method.Links.Add(new Link(conditionalBranch.TrueSlot, writeTrue));
            method.Links.Add(new Link(conditionalBranch.FalseSlot, writeFalse));

            visualScript.Methods.Add(method);

            // Test
            TestAndCompareOutput(visualScript, "True", testInstance =>
            {
                testInstance.Condition = true;
                testInstance.Test();
            });

            TestAndCompareOutput(visualScript, "False", testInstance =>
            {
                testInstance.Condition = false;
                testInstance.Test();
            });
        }

        [Test]
        public void TestVariableSet()
        {
            var visualScript = new VisualScriptAsset();

            var condition = new Property("bool", "Condition");
            visualScript.Properties.Add(condition);

            // Build blocks
            // TODO: Switch to a simple Write(variable) later, so that we don't depend on ConditionalBranchBlock for this test?
            var functionStart = new FunctionStartBlock();
            var conditionGet = new VariableGet { Name = condition.Name };
            var conditionSet = new VariableSet { Name = condition.Name };
            var conditionalBranch = new ConditionalBranchBlock();
            var writeTrue = new CustomCodeBlock { Code = "System.Console.Write(true);" };
            var writeFalse = new CustomCodeBlock { Code = "System.Console.Write(false);" };
            var method = new Method();
            method.Blocks.Add(functionStart);
            method.Blocks.Add(conditionGet);
            method.Blocks.Add(conditionSet);
            method.Blocks.Add(conditionalBranch);
            method.Blocks.Add(writeTrue);
            method.Blocks.Add(writeFalse);

            // Generate slots
            foreach (var block in method.Blocks)
                block.GenerateSlots(block.Slots, new SlotGeneratorContext());

            // Build links
            method.Links.Add(new Link(functionStart, conditionSet));
            method.Links.Add(new Link(conditionSet, conditionalBranch));
            method.Links.Add(new Link(conditionGet.ValueSlot, conditionalBranch.ConditionSlot));
            method.Links.Add(new Link(conditionalBranch.TrueSlot, writeTrue));
            method.Links.Add(new Link(conditionalBranch.FalseSlot, writeFalse));

            visualScript.Methods.Add(method);

            // Test
            conditionSet.InputSlot.Value = "true";
            TestAndCompareOutput(visualScript, "True", testInstance =>
            {
                testInstance.Test();
            });

            conditionSet.InputSlot.Value = "false";
            TestAndCompareOutput(visualScript, "False", testInstance =>
            {
                testInstance.Test();
            });
        }

        private static void TestAndCompareOutput(VisualScriptAsset visualScriptAsset, string expectedOutput, Action<dynamic> testCode)
        {
            // Compile
            var compilerResult = VisualScriptCompiler.Generate(visualScriptAsset, new VisualScriptCompilerOptions
            {
                Class = "TestClass",
            });

            using (var textWriter = new StringWriter())
            {
                Console.SetOut(textWriter);

                // Create class
                var testInstance = CreateInstance(new[] { compilerResult.SyntaxTree });
                // Execute method
                testCode(testInstance);

                // Check output
                textWriter.Flush();
                Assert.That(textWriter.ToString(), Is.EqualTo(expectedOutput));

                // Restore Console.Out
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }

        private static dynamic CreateInstance(SyntaxTree[] syntaxTrees)
        {
            var compilation = CSharpCompilation.Create("Test.dll",
                syntaxTrees,
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var peStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(peStream, pdbStream);

                if (!emitResult.Success)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Compilation errors:");
                    foreach (var diagnostic in emitResult.Diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Error))
                    {
                        sb.AppendLine(diagnostic.ToString());
                    }

                    throw new InvalidOperationException(sb.ToString());
                }

                peStream.Position = 0;
                pdbStream.Position = 0;

                var assembly = Assembly.Load(peStream.ToArray(), pdbStream.ToArray());
                var @class = assembly.GetType("TestClass");
                return Activator.CreateInstance(@class);
            }
        }
    }
}