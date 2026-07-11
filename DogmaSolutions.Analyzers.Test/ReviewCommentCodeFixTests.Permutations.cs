using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable All

namespace DogmaSolutions.Analyzers.Test;

/// <summary>
/// Review-comment code-fix tests for every FixKind and NoFix permutation
/// across DSA005, DSA017, DSA027, and DSA029.
/// </summary>
public partial class ReviewCommentCodeFixTests
{
   private const string Dsa017ReviewKey = DSA017Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;

   // ---------------------------------------------------------------
   // DSA017: FixKind permutations — primary fix IS available
   // ---------------------------------------------------------------

   private static IEnumerable<object[]> GetDSA017FixKindReviewCommentCases =>
   [
      [
         "SetAddReturnsBool: HashSet Contains/Add",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            {|#0:if (!set.Contains(item))
            {
                set.Add(item);
            }|}
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!set.Contains(item))
            {
                set.Add(item);
            }|}
        }
    }
}",
         "HashSet",
         "Add (already returns a bool indicating whether the element was added)"
      ],
      [
         "DictionaryTryAddThrowPattern: Dictionary ContainsKey/throw + Add",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int value)
        {
            {|#0:if (store.ContainsKey(key))
                throw new InvalidOperationException();|}
            store.Add(key, value);
        }
    }
}",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int value)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (store.ContainsKey(key))
                throw new InvalidOperationException();|}
            store.Add(key, value);
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "SetAddThrowPattern: HashSet Contains/throw + Add",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            {|#0:if (set.Contains(item))
                throw new InvalidOperationException();|}
            set.Add(item);
        }
    }
}",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (set.Contains(item))
                throw new InvalidOperationException();|}
            set.Add(item);
        }
    }
}",
         "HashSet",
         "Add (already returns a bool indicating whether the element was added)"
      ],
      [
         "DictionaryTryAdd via Pattern C: positive check + else with insert",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int value)
        {
            {|#0:if (store.ContainsKey(key))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                store.Add(key, value);
            }|}
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int value)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (store.ContainsKey(key))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                store.Add(key, value);
            }|}
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "SetAddReturnsBool via Pattern C: HashSet positive check + else with insert",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            {|#0:if (set.Contains(item))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                set.Add(item);
            }|}
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(HashSet<string> set, string item)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (set.Contains(item))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                set.Add(item);
            }|}
        }
    }
}",
         "HashSet",
         "Add (already returns a bool indicating whether the element was added)"
      ],
   ];

   [TestMethod]
   [DynamicData(nameof(GetDSA017FixKindReviewCommentCases))]
   public async Task ReviewComment_DSA017_FixKindPermutations(
      string title, string source, string fixedSource, string typeName, string suggestion)
   {
      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa017ReviewKey;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments(typeName, suggestion));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments(typeName, suggestion));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   // ---------------------------------------------------------------
   // DSA017: NoFix permutations — no primary fix, review comment only
   // ---------------------------------------------------------------

   private static IEnumerable<object[]> GetDSA017NoFixReviewCommentCases =>
   [
      [
         "TryGetValue else braces, out var unused",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, string> target, Dictionary<string, string> other)
        {
            foreach (var entry in other)
            {
                {|#0:if (!target.TryGetValue(entry.Key, out var value))
                {
                    target.Add(entry.Key, entry.Value);
                }
                else
                {
                    target[entry.Key] = entry.Value;
                }|}
            }
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, string> target, Dictionary<string, string> other)
        {
            foreach (var entry in other)
            {
                /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
                 * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
                 * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
                 */
                {|#0:if (!target.TryGetValue(entry.Key, out var value))
                {
                    target.Add(entry.Key, entry.Value);
                }
                else
                {
                    target[entry.Key] = entry.Value;
                }|}
            }
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "TryGetValue else no braces, out var unused",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, string> target, Dictionary<string, string> other)
        {
            foreach (var entry in other)
            {
                {|#0:if (!target.TryGetValue(entry.Key, out var value))
                    target.Add(entry.Key, entry.Value);
                else
                    target[entry.Key] = entry.Value;|}
            }
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, string> target, Dictionary<string, string> other)
        {
            foreach (var entry in other)
            {
                /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
                 * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
                 * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
                 */
                {|#0:if (!target.TryGetValue(entry.Key, out var value))
                    target.Add(entry.Key, entry.Value);
                else
                    target[entry.Key] = entry.Value;|}
            }
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "TryGetValue no braces, out var used in else",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            {|#0:if (!store.TryGetValue(key, out var existing))
                store.Add(key, newValue);
            else
                store[key] = existing + newValue;|}
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!store.TryGetValue(key, out var existing))
                store.Add(key, newValue);
            else
                store[key] = existing + newValue;|}
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "TryGetValue no braces, out var used after if",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            {|#0:if (!store.TryGetValue(key, out var existing))
                store.Add(key, newValue);|}
            System.Console.WriteLine(existing);
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!store.TryGetValue(key, out var existing))
                store.Add(key, newValue);|}
            System.Console.WriteLine(existing);
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "TryGetValue pre-declared out var used after if",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            int existing;
            {|#0:if (!store.TryGetValue(key, out existing))
            {
                store.Add(key, newValue);
            }|}
            System.Console.WriteLine(existing);
        }
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            int existing;
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!store.TryGetValue(key, out existing))
            {
                store.Add(key, newValue);
            }|}
            System.Console.WriteLine(existing);
        }
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "Non-adjacent throw and add",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            {|#0:if (store.ContainsKey(key))
                throw new InvalidOperationException();|}
            var value = ComputeValue(key);
            store.Add(key, value);
        }
        private int ComputeValue(string key) => 42;
    }
}",
         @"
using System;
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (store.ContainsKey(key))
                throw new InvalidOperationException();|}
            var value = ComputeValue(key);
            store.Add(key, value);
        }
        private int ComputeValue(string key) => 42;
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "Complex else body (Pattern C)",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            {|#0:if (store.ContainsKey(key))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                var value = ComputeValue(key);
                store.Add(key, value);
            }|}
        }
        private int ComputeValue(string key) => 42;
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (store.ContainsKey(key))
            {
                System.Console.WriteLine(""exists"");
            }
            else
            {
                var value = ComputeValue(key);
                store.Add(key, value);
            }|}
        }
        private int ComputeValue(string key) => 42;
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
      [
         "Complex if body (non-simple insert)",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            {|#0:if (!store.ContainsKey(key))
            {
                var value = ComputeExpensiveValue(key);
                store.Add(key, value);
            }|}
        }
        private int ComputeExpensiveValue(string key) => 42;
    }
}",
         @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!store.ContainsKey(key))
            {
                var value = ComputeExpensiveValue(key);
                store.Add(key, value);
            }|}
        }
        private int ComputeExpensiveValue(string key) => 42;
    }
}",
         "Dictionary",
         "TryAdd or indexer assignment [key] = value"
      ],
   ];

   [TestMethod]
   [DynamicData(nameof(GetDSA017NoFixReviewCommentCases))]
   public async Task ReviewComment_DSA017_NoFixPermutations(
      string title, string source, string fixedSource, string typeName, string suggestion)
   {
      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa017ReviewKey;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments(typeName, suggestion));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments(typeName, suggestion));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   // ---------------------------------------------------------------
   // DSA005: Stopwatch path — review comment alongside Stopwatch fix
   // ---------------------------------------------------------------

   [TestMethod]
   public async Task ReviewComment_DSA005_WithStopwatchFix()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        private void DoWork() {}

        {|#0:public void Test()
        {
            var operationStart = DateTime.UtcNow;
            DoWork();
            var operationEnd = DateTime.UtcNow;
            var elapsed = operationEnd - operationStart;
        }|}
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        private void DoWork() {}

        /* [DSA005 / CySec + QA + Code Smell]: Potential non-deterministic point-in-time execution (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA005.md)
         * CWE-361: 7PK - Time and State - https://cwe.mitre.org/data/definitions/361.html
         * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
         */
        {|#0:public void Test()
        {
            var operationStart = DateTime.UtcNow;
            DoWork();
            var operationEnd = DateTime.UtcNow;
            var elapsed = operationEnd - operationStart;
        }|}
    }
}";

      var test = new CSharpCodeFixVerifier<DSA005Analyzer, DSA005CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA005Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA005Analyzer, DSA005CodeFixProvider>.Diagnostic(DSA005Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA005Analyzer, DSA005CodeFixProvider>.Diagnostic(DSA005Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   // ---------------------------------------------------------------
   // DSA029: int property — review comment alongside Range + Remove
   // ---------------------------------------------------------------

   [TestMethod]
   public async Task ReviewComment_DSA029_IntPropertyWithRangeFix()
   {
      var source = @"
using System;
using System.ComponentModel.DataAnnotations;
namespace TestApp
{
    public class MyClass
    {
        {|#0:[Required] public int Count { get; set; }|}
    }
}";

      var fixedSource = @"
using System;
using System.ComponentModel.DataAnnotations;
namespace TestApp
{
    public class MyClass
    {
        /* [DSA029 / CySec + QA + Bug]: The `RequiredAttribute` has no impact on a not-nullable value type (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA029.md)
         * `RequiredAttribute` - https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute
         * `RangeAttribute` - https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute
         * Value types - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types
         * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
         * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
         */
        {|#0:[Required] public int Count { get; set; }|}
    }
}";

      var test = new CSharpCodeFixVerifier<DSA029Analyzer, DSA029CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA029Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA029Analyzer, DSA029CodeFixProvider>.Diagnostic(DSA029Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA029Analyzer, DSA029CodeFixProvider>.Diagnostic(DSA029Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }
}
