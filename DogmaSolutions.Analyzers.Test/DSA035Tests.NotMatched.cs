using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DogmaSolutions.Analyzers.Test;

public partial class DSA035Tests
{
    private static IEnumerable<object[]> GetNotMatchedCases =>
    [
        // ── Receiver IS the loop iteration variable ─────────────────────────

        [
            "GetType() on foreach iteration variable",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(List<object> items)
                    {
                        foreach (var item in items)
                        {
                            var t = item.GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on for loop indexer expression",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object[] items)
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            var t = items[i].GetType();
                        }
                    }
                }
            }"
        ],

        // ── Receiver IS modified inside the loop ────────────────────────────

        [
            "GetType() on variable reassigned inside loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object[] objects)
                    {
                        object current = objects[0];
                        for (int i = 0; i < objects.Length; i++)
                        {
                            current = objects[i];
                            var t = current.GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on variable modified by simple assignment in loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test()
                    {
                        object obj = ""hello"";
                        for (int i = 0; i < 100; i++)
                        {
                            var t = obj.GetType();
                            obj = i.ToString();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on variable modified by compound assignment in loop",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type)
                    {
                        string name = """";
                        for (int i = 0; i < 100; i++)
                        {
                            name += type.Name;
                            var t = name.GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on variable modified by ref in loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    private void Swap(ref object o) { o = new object(); }
                    public void Test()
                    {
                        object obj = new object();
                        for (int i = 0; i < 100; i++)
                        {
                            var t = obj.GetType();
                            Swap(ref obj);
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on variable modified by out in loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    private void Create(out object o) { o = new object(); }
                    public void Test()
                    {
                        object obj = new object();
                        for (int i = 0; i < 100; i++)
                        {
                            var t = obj.GetType();
                            Create(out obj);
                        }
                    }
                }
            }"
        ],

        // ── Argument IS the loop variable ───────────────────────────────────

        [
            "GetProperty() with foreach iteration variable as argument",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<string> propNames)
                    {
                        foreach (var name in propNames)
                        {
                            var prop = type.GetProperty(name);
                        }
                    }
                }
            }"
        ],
        [
            "GetMethod() with loop variable in string concatenation as argument",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var prop = type.GetProperty(""Prop"" + i);
                        }
                    }
                }
            }"
        ],
        [
            "GetField() with argument modified inside loop",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, string[] fieldNames)
                    {
                        string fieldName = fieldNames[0];
                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            fieldName = fieldNames[i];
                            var field = type.GetField(fieldName);
                        }
                    }
                }
            }"
        ],
        [
            "IsInstanceOfType() with loop iteration variable as argument",
            @"
            using System;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<object> items)
                    {
                        foreach (var item in items)
                        {
                            var result = type.IsInstanceOfType(item);
                        }
                    }
                }
            }"
        ],

        // ── Non-reflection method with same name ────────────────────────────

        [
            "Non-reflection method named GetType with string parameter",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class CustomService
                {
                    public string GetType(string key) => key;
                }
                public class MyClass
                {
                    public void Test(CustomService svc, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var t = svc.GetType(""key"");
                        }
                    }
                }
            }"
        ],
        [
            "Non-reflection method named GetProperties on custom class",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class CustomMapper
                {
                    public List<string> GetProperties() => new();
                }
                public class MyClass
                {
                    public void Test(CustomMapper mapper, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = mapper.GetProperties();
                        }
                    }
                }
            }"
        ],

        // ── Not inside any loop ─────────────────────────────────────────────

        [
            "Reflection call not inside any loop",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object obj)
                    {
                        var t = obj.GetType();
                    }
                }
            }"
        ],

        // ── Nested loop: receiver IS inner loop variable ────────────────────

        [
            "GetType() inside nested loop on inner loop variable",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(List<object> outer, List<object> inner)
                    {
                        foreach (var a in outer)
                        {
                            foreach (var b in inner)
                            {
                                var t = b.GetType();
                            }
                        }
                    }
                }
            }"
        ],

        // ── Receiver from non-invariant source ──────────────────────────────

        [
            "GetType() on result of method call (non-invariant receiver)",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    private object GetNext(int i) => new object();
                    public void Test()
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var t = GetNext(i).GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on result of parameterless method call (could have side effects)",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    private object GetCurrent() => new object();
                    public void Test()
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var t = GetCurrent().GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on new object created inside loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test()
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var t = new object().GetType();
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on await result inside loop",
            @"
            using System.Threading.Tasks;
            namespace TestApp
            {
                public class MyClass
                {
                    private Task<object> GetAsync() => Task.FromResult(new object());
                    public async Task Test()
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var t = (await GetAsync()).GetType();
                        }
                    }
                }
            }"
        ],

        // ── Methods NOT in the reflection method set ────────────────────────

        [
            "PropertyInfo.GetValue() is not in the reflection method set",
            @"
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object target, PropertyInfo[] properties)
                    {
                        foreach (var prop in properties)
                        {
                            var val = prop.GetValue(target);
                        }
                    }
                }
            }"
        ],
        [
            "MethodInfo.Invoke() is not in the reflection method set",
            @"
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(MethodInfo method, object target, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var result = method.Invoke(target, null);
                        }
                    }
                }
            }"
        ],
        [
            "FieldInfo.SetValue() is not in the reflection method set",
            @"
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(FieldInfo field, object target, int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            field.SetValue(target, i);
                        }
                    }
                }
            }"
        ],

        // ── typeof() alone is not a reflection call ─────────────────────────

        [
            "typeof expression in loop is not a reflection invocation",
            @"
            using System;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var t = typeof(string);
                        }
                    }
                }
            }"
        ],

        // ── Tuple deconstruction foreach: iteration variable ────────────────

        [
            "GetType() on tuple deconstruction iteration variable",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test()
                    {
                        var items = new List<(object Obj, int Value)> { (new object(), 1) };
                        foreach (var (obj, value) in items)
                        {
                            var t = obj.GetType();
                        }
                    }
                }
            }"
        ],

        // ── Reflection on non-const field (field may change) ────────────────

        [
            "GetProperties() on non-const instance field (may change between iterations)",
            @"
            using System;
            using System.Reflection;
            namespace TestApp
            {
                public class MyClass
                {
                    private Type _type = typeof(string);
                    public void Test(int[] arr)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var props = _type.GetProperties();
                        }
                    }
                }
            }"
        ],

        // ── Receiver is variable declared inside loop ───────────────────────

        [
            "GetType() on variable declared and assigned inside loop",
            @"
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(object[] items)
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            var local = items[i];
                            var t = local.GetType();
                        }
                    }
                }
            }"
        ],

        // ── Chained call where middle part uses loop variable ───────────────

        [
            "Chained reflection where GetProperty uses loop variable argument",
            @"
            using System;
            using System.Reflection;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(Type type, List<string> propNames)
                    {
                        foreach (var name in propNames)
                        {
                            var attr = type.GetProperty(name)?.GetCustomAttributes(true);
                        }
                    }
                }
            }"
        ],

        // ── Stacked foreach without braces ──────────────────────────────────

        [
            "Stacked foreach without braces: inner iteration variable used as receiver",
            @"
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test()
                    {
                        var outer = new List<object> { new object() };
                        var inner = new List<object> { new object() };
                        foreach (var a in outer)
                        foreach (var b in inner)
                        {
                            var t = b.GetType();
                        }
                    }
                }
            }"
        ],

        // ── Lambda/delegate/local-function parameter inside outer loop ─

        [
            "GetType() on lambda parameter inside outer foreach loop",
            @"
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using System.Collections.Concurrent;
            using System.Collections.Generic;
            namespace TestApp
            {
                public interface IProcessor
                {
                    Task<List<string>> ProcessAsync(string target, CancellationToken ct);
                }
                public class MyClass
                {
                    private readonly List<IProcessor> _processors = new();
                    public async Task RunAsync(List<string> targets, CancellationToken cancellationToken)
                    {
                        foreach (var target in targets)
                        {
                            var results = new ConcurrentBag<string>();
                            await Parallel.ForEachAsync(
                                _processors,
                                cancellationToken,
                                async (processor, ct) =>
                                {
                                    try
                                    {
                                        var found = await processor.ProcessAsync(target, ct).ConfigureAwait(false);
                                        foreach (var item in found)
                                        {
                                            results.Add(item);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(processor.GetType().Name + "": "" + ex.Message);
                                    }
                                });
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on simple lambda parameter inside for loop",
            @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(List<List<object>> batches)
                    {
                        for (int i = 0; i < batches.Count; i++)
                        {
                            batches[i].ForEach(item =>
                            {
                                var t = item.GetType();
                            });
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on anonymous method parameter inside while loop",
            @"
            using System;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(List<object> items)
                    {
                        int i = 0;
                        while (i < 10)
                        {
                            items.ForEach(delegate(object item)
                            {
                                var t = item.GetType();
                            });
                            i++;
                        }
                    }
                }
            }"
        ],
        [
            "GetType() on local function parameter inside foreach loop",
            @"
            using System;
            using System.Collections.Generic;
            namespace TestApp
            {
                public class MyClass
                {
                    public void Test(List<string> names)
                    {
                        foreach (var name in names)
                        {
                            void ProcessItem(object item)
                            {
                                var t = item.GetType();
                            }
                            ProcessItem(name);
                        }
                    }
                }
            }"
        ],
    ];

    [TestMethod]
    [DynamicData(nameof(GetNotMatchedCases), DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public async Task NotMatched(string title, string sourceCode)
    {
        var test = new CSharpAnalyzerVerifier<DSA035Analyzer>.Test();
        test.TestCode = sourceCode;
        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;

        await test.RunAsync().ConfigureAwait(false);
    }
}
