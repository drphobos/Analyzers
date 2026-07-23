using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DogmaSolutions.Analyzers.Test;

public partial class DSA035Tests
{
    private static IEnumerable<object[]> GetMatchedCases =>
    [
        // ── Loop type × receiver type combinations ──────────────────────────

        [
            "GetType() on parameter in for loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var t = {|#0:obj.GetType()|};
                        }
                    }
                }
            }",
            "obj.GetType()"
        ],
        [
            "GetType() on parameter in foreach loop",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object target, List<string> items)
                    {
                        foreach (var item in items)
                        {
                            var t = {|#0:target.GetType()|};
                        }
                    }
                }
            }",
            "target.GetType()"
        ],
        [
            "GetType() on parameter in while loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj)
                    {
                        int i = 0;
                        while (i < 100)
                        {
                            var t = {|#0:obj.GetType()|};
                            i++;
                        }
                    }
                }
            }",
            "obj.GetType()"
        ],
        [
            "GetType() on parameter in do-while loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj)
                    {
                        int i = 0;
                        do
                        {
                            var t = {|#0:obj.GetType()|};
                            i++;
                        } while (i < 100);
                    }
                }
            }",
            "obj.GetType()"
        ],
        [
            "GetType() on loop-invariant local variable",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj)
                    {
                        var local = obj;
                        for (int i = 0; i < 100; i++)
                        {
                            var t = {|#0:local.GetType()|};
                        }
                    }
                }
            }",
            "local.GetType()"
        ],
        [
            "GetType() on this in for loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var t = {|#0:this.GetType()|};
                        }
                    }
                }
            }",
            "this.GetType()"
        ],

        // ── Reflection method coverage ──────────────────────────────────────

        [
            "GetProperties() on loop-invariant Type in foreach",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<string> names)
                    {
                        foreach (var name in names)
                        {
                            var props = {|#0:type.GetProperties()|};
                        }
                    }
                }
            }",
            "type.GetProperties()"
        ],
        [
            "GetProperty() with constant string argument in while loop",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type)
                    {
                        int i = 0;
                        while (i < 100)
                        {
                            var prop = {|#0:type.GetProperty(""Name"")|};
                            i++;
                        }
                    }
                }
            }",
            @"type.GetProperty(""Name"")"
        ],
        [
            "GetMethod() on loop-invariant Type in do-while loop",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type)
                    {
                        int i = 0;
                        do
                        {
                            var method = {|#0:type.GetMethod(""Execute"")|};
                            i++;
                        } while (i < 100);
                    }
                }
            }",
            @"type.GetMethod(""Execute"")"
        ],
        [
            "GetMethods() with BindingFlags on loop-invariant Type",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var methods = {|#0:type.GetMethods(BindingFlags.Public | BindingFlags.Instance)|};
                        }
                    }
                }
            }",
            "type.GetMethods(BindingFlags.Public | BindingFlags.Instance)"
        ],
        [
            "GetFields() on loop-invariant variable in for loop",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int count)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var fields = {|#0:type.GetFields()|};
                        }
                    }
                }
            }",
            "type.GetFields()"
        ],
        [
            "GetField() with constant string argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var field = {|#0:type.GetField(""_value"")|};
                        }
                    }
                }
            }",
            @"type.GetField(""_value"")"
        ],
        [
            "GetMembers() on loop-invariant Type in for loop",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int count)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var members = {|#0:type.GetMembers()|};
                        }
                    }
                }
            }",
            "type.GetMembers()"
        ],
        [
            "GetMember() with constant string argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var member = {|#0:type.GetMember(""Name"")|};
                        }
                    }
                }
            }",
            @"type.GetMember(""Name"")"
        ],
        [
            "GetConstructors() on loop-invariant Type in foreach",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<int> items)
                    {
                        foreach (var item in items)
                        {
                            var ctors = {|#0:type.GetConstructors()|};
                        }
                    }
                }
            }",
            "type.GetConstructors()"
        ],
        [
            "GetConstructor() with constant Type array argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var ctor = {|#0:type.GetConstructor(Type.EmptyTypes)|};
                        }
                    }
                }
            }",
            "type.GetConstructor(Type.EmptyTypes)"
        ],
        [
            "GetInterfaces() on loop-invariant Type",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var ifaces = {|#0:type.GetInterfaces()|};
                        }
                    }
                }
            }",
            "type.GetInterfaces()"
        ],
        [
            "GetInterface() with constant string argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var iface = {|#0:type.GetInterface(""IDisposable"")|};
                        }
                    }
                }
            }",
            @"type.GetInterface(""IDisposable"")"
        ],
        [
            "GetEvents() on loop-invariant Type",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var events = {|#0:type.GetEvents()|};
                        }
                    }
                }
            }",
            "type.GetEvents()"
        ],
        [
            "GetEvent() with constant string argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var evt = {|#0:type.GetEvent(""Click"")|};
                        }
                    }
                }
            }",
            @"type.GetEvent(""Click"")"
        ],
        [
            "GetNestedTypes() on loop-invariant Type",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var nested = {|#0:type.GetNestedTypes()|};
                        }
                    }
                }
            }",
            "type.GetNestedTypes()"
        ],
        [
            "GetNestedType() with constant string argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var nested = {|#0:type.GetNestedType(""Inner"")|};
                        }
                    }
                }
            }",
            @"type.GetNestedType(""Inner"")"
        ],
        [
            "GetCustomAttributes() on loop-invariant Type in foreach",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<int> items)
                    {
                        foreach (var item in items)
                        {
                            var attrs = {|#0:type.GetCustomAttributes(true)|};
                        }
                    }
                }
            }",
            "type.GetCustomAttributes(true)"
        ],
        [
            "GetGenericArguments() on loop-invariant Type",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var args = {|#0:type.GetGenericArguments()|};
                        }
                    }
                }
            }",
            "type.GetGenericArguments()"
        ],
        [
            "IsAssignableFrom() with loop-invariant argument",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type baseType, Type derivedType, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var result = {|#0:baseType.IsAssignableFrom(derivedType)|};
                        }
                    }
                }
            }",
            "baseType.IsAssignableFrom(derivedType)"
        ],
        [
            "IsSubclassOf() with loop-invariant argument",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type derivedType, Type baseType, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var result = {|#0:derivedType.IsSubclassOf(baseType)|};
                        }
                    }
                }
            }",
            "derivedType.IsSubclassOf(baseType)"
        ],
        [
            "IsInstanceOfType() with loop-invariant argument",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, object obj, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var result = {|#0:type.IsInstanceOfType(obj)|};
                        }
                    }
                }
            }",
            "type.IsInstanceOfType(obj)"
        ],

        // ── Invariant argument variations ───────────────────────────────────

        [
            "GetProperty() with loop-invariant parameter argument in foreach",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, string propName, List<int> items)
                    {
                        foreach (var item in items)
                        {
                            var prop = {|#0:type.GetProperty(propName)|};
                        }
                    }
                }
            }",
            "type.GetProperty(propName)"
        ],
        [
            "GetProperty() with loop-invariant local variable argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        var propName = ""Name"";
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var prop = {|#0:type.GetProperty(propName)|};
                        }
                    }
                }
            }",
            "type.GetProperty(propName)"
        ],

        // ── Receiver type: typeof() ──────────────────────────────────────────

        [
            "GetProperties() on typeof() expression",
            @"
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = {|#0:typeof(string).GetProperties()|};
                        }
                    }
                }
            }",
            "typeof(string).GetProperties()"
        ],

        // ── Receiver type: readonly/static field ─────────────────────────────

        [
            "GetProperties() on readonly field receiver",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    private readonly Type _type = typeof(string);
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = {|#0:_type.GetProperties()|};
                        }
                    }
                }
            }",
            "_type.GetProperties()"
        ],
        [
            "GetMethods() on static readonly field receiver",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    private static readonly Type TargetType = typeof(string);
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var methods = {|#0:TargetType.GetMethods()|};
                        }
                    }
                }
            }",
            "TargetType.GetMethods()"
        ],

        // ── Receiver type: property ──────────────────────────────────────────

        [
            "GetProperties() on property receiver",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class Config
                {
                    public Type TargetType { get; set; }
                }
                public class MyClass
                {
                    public void Test(Config config, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = {|#0:config.TargetType.GetProperties()|};
                        }
                    }
                }
            }",
            "config.TargetType.GetProperties()"
        ],

        // ── Nested loop: invariant from outer scope ──────────────────────────

        [
            "GetType() in inner loop on invariant receiver from outer scope",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            for (int x = 0; x < 10; x++)
                            {
                                var t = {|#0:obj.GetType()|};
                            }
                        }
                    }
                }
            }",
            "obj.GetType()"
        ],

        // ── Foreach with deconstruction ──────────────────────────────────────

        [
            "GetType() in foreach with deconstruction on invariant receiver",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object target)
                    {
                        var tuples = new List<(int X, int Y)> { (1,2), (3,4) };
                        foreach (var (x, y) in tuples)
                        {
                            var t = {|#0:target.GetType()|};
                        }
                    }
                }
            }",
            "target.GetType()"
        ],

        // ── Reflection inside conditional inside loop ────────────────────────

        [
            "GetType() inside if block inside loop on invariant receiver",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (i > 5)
                            {
                                var t = {|#0:obj.GetType()|};
                            }
                        }
                    }
                }
            }",
            "obj.GetType()"
        ],

        // ── Reflection inside try/catch inside loop ──────────────────────────

        [
            "GetType() inside try block inside loop on invariant receiver",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            try
                            {
                                var t = {|#0:obj.GetType()|};
                            }
                            catch { }
                        }
                    }
                }
            }",
            "obj.GetType()"
        ],
    ];

    [TestMethod]
    [DynamicData(nameof(GetMatchedCases), DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public async Task Matched(string title, string sourceCode, string expectedExpression)
    {
        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = sourceCode;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments(expectedExpression));

        await test.RunAsync().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ChainedReflectionCallsOnInvariantReceiver_BothFlagged()
    {
        var source = @"
            using System.Reflection;
            using System.Collections.Generic;
            using System.Linq;
            namespace TestApp
            {
                public class DataRecord
                {
                    public Dictionary<string, object> Metadata { get; } = new();
                    public string Name { get; set; }
                }
                public class MyClass
                {
                    public void Test(DataRecord record, PropertyInfo[] knownProperties)
                    {
                        foreach (var property in knownProperties)
                        {
                            if (record.Metadata.ContainsKey(property.Name))
                                continue;

                            if ({|#1:{|#0:record.GetType()|}.GetProperties()|}.Any(p => p.Name == property.Name))
                                continue;
                        }
                    }
                }
            }";

        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = source;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("record.GetType()"));
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("record.GetType().GetProperties()"));

        await test.RunAsync().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task MultipleReflectionCallsInSameLoop_BothFlagged()
    {
        var source = @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = {|#0:type.GetProperties()|};
                            var methods = {|#1:type.GetMethods()|};
                        }
                    }
                }
            }";

        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = source;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("type.GetProperties()"));
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("type.GetMethods()"));

        await test.RunAsync().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task MultipleDifferentReflectionMethods_AllFlagged()
    {
        var source = @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = {|#0:type.GetProperties()|};
                            var fields = {|#1:type.GetFields()|};
                            var ctors = {|#2:type.GetConstructors()|};
                        }
                    }
                }
            }";

        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = source;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("type.GetProperties()"));
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(1)
                .WithArguments("type.GetFields()"));
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(2)
                .WithArguments("type.GetConstructors()"));

        await test.RunAsync().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task GetTypeOnInvariantInsideLambdaInsideLoop_Flagged()
    {
        var source = @"
            using System;
            using System.Linq;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var t = {|#0:obj.GetType()|}.Name;
                        }
                    }
                }
            }";

        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = source;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<DSA035Analyzer>.Diagnostic(DSA035Analyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("obj.GetType()"));

        await test.RunAsync().ConfigureAwait(false);
    }
}
