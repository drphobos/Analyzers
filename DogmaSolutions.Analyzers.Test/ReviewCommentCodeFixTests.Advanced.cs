using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable All

namespace DogmaSolutions.Analyzers.Test;

public partial class ReviewCommentCodeFixTests
{

   [TestMethod]
   public async Task TabIndentation()
   {
      var source = @"
using System;
namespace TestApp
{
	public class MyClass
	{
		public bool Check(string s)
		{
			return {|#0:string.IsNullOrEmpty(s)|};
		}
	}
}";

      var fixedSource = @"
using System;
namespace TestApp
{
	public class MyClass
	{
		public bool Check(string s)
		{
			/* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
			 * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
			 * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
			 */
			return {|#0:string.IsNullOrEmpty(s)|};
		}
	}
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ExpressionBodiedMember_DSA004()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public DateTime Timestamp => {|#0:DateTime.Now|};
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        /* [DSA004 / CySec + QA + Code Smell]: Use `DateTime.UtcNow` instead of `DateTime.Now` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA004.md)
         * CWE-361: 7PK - Time and State - https://cwe.mitre.org/data/definitions/361.html
         * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
         */
        public DateTime Timestamp => {|#0:DateTime.Now|};
    }
}";

      var test = new CSharpCodeFixVerifier<DSA004Analyzer, DSA004CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA004Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA004Analyzer, DSA004CodeFixProvider>.Diagnostic(DSA004Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA004Analyzer, DSA004CodeFixProvider>.Diagnostic(DSA004Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task PreservesRegionDirective()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s)
        {
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s)
        {
            /* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
             * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task FirstStatementInMethod_NoLeadingTrivia()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s) {
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s) {
            /* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
             * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task MultipleStatementsOnSameMethod()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s)
        {
            var x = 1;
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s)
        {
            var x = 1;
            /* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
             * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Idempotency_DoesNotInsertDuplicateComment()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public bool Check(string s)
        {
            /* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
             * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = source;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ReviewComment_DSA005()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        private void DoWork(DateTime dt) {}
        private void DoOther(DateTime dt) {}

        {|#0:public void Test()
        {
            DoWork(DateTime.UtcNow);
            DoOther(DateTime.UtcNow);
        }|}
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        private void DoWork(DateTime dt) {}
        private void DoOther(DateTime dt) {}

        /* [DSA005 / CySec + QA + Code Smell]: Potential non-deterministic point-in-time execution (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA005.md)
         * CWE-361: 7PK - Time and State - https://cwe.mitre.org/data/definitions/361.html
         * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
         */
        {|#0:public void Test()
        {
            DoWork(DateTime.UtcNow);
            DoOther(DateTime.UtcNow);
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

   [TestMethod]
   public async Task ReviewComment_DSA017()
   {
      var source = @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int value)
        {
            {|#0:if (!store.ContainsKey(key))
            {
                store.Add(key, value);
            }|}
        }
    }
}";

      var fixedSource = @"
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
            {|#0:if (!store.ContainsKey(key))
            {
                store.Add(key, value);
            }|}
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA017Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ReviewComment_DSA027()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public string Build(string[] items)
        {
            string result = """";
            foreach (var item in items)
            {
                {|#0:result += item|};
            }
            return result;
        }
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        public string Build(string[] items)
        {
            string result = """";
            foreach (var item in items)
            {
                /* [DSA027 / CySec + QA + Performance]: Replace string concatenation in loops with `StringBuilder` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA027.md)
                 * MITRE, CWE-400: Uncontrolled Resource Consumption - https://cwe.mitre.org/data/definitions/400.html
                 * MITRE, CWE-789: Memory Allocation with Excessive Size Value - https://cwe.mitre.org/data/definitions/789.html
                 * CA1834: Use StringBuilder.Append(char) when applicable - https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1834
                 * String concatenation (C# guide) - https://learn.microsoft.com/en-us/dotnet/csharp/how-to/concatenate-multiple-strings
                 * `StringBuilder` Class - https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder
                 */
                {|#0:result += item|};
            }
            return result;
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA027Analyzer, DSA027CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA027Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA027Analyzer, DSA027CodeFixProvider>.Diagnostic(DSA027Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("result"));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA027Analyzer, DSA027CodeFixProvider>.Diagnostic(DSA027Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("result"));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ReviewComment_DSA029()
   {
      var source = @"
using System;
using System.ComponentModel.DataAnnotations;
namespace TestApp
{
    public class MyClass
    {
        {|#0:[Required] public bool IsActive { get; set; }|}
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
        {|#0:[Required] public bool IsActive { get; set; }|}
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

   [TestMethod]
   public async Task ReviewComment_DSA017_WhenPrimaryFixUnavailable_OutVarUsedInElse()
   {
      var source = @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            {|#0:if (!store.TryGetValue(key, out var existing))
            {
                store.Add(key, newValue);
            }
            else
            {
                store[key] = existing + newValue;
            }|}
        }
    }
}";

      var fixedSource = @"
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
            {
                store.Add(key, newValue);
            }
            else
            {
                store[key] = existing + newValue;
            }|}
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA017Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ReviewComment_DSA017_WhenPrimaryFixUnavailable_OutVarUsedAfterIf()
   {
      var source = @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(Dictionary<string, int> store, string key, int newValue)
        {
            {|#0:if (!store.TryGetValue(key, out var existing))
            {
                store.Add(key, newValue);
            }|}
            System.Console.WriteLine(existing);
        }
    }
}";

      var fixedSource = @"
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
            {
                store.Add(key, newValue);
            }|}
            System.Console.WriteLine(existing);
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA017Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("Dictionary", "TryAdd or indexer assignment [key] = value"));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task ReviewComment_DSA017_WhenPrimaryFixUnavailable_SortedListNoAtomicAlternative()
   {
      var source = @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(SortedList<string, int> items, string key, int value)
        {
            {|#0:if (!items.ContainsKey(key))
            {
                items.Add(key, value);
            }|}
        }
    }
}";

      var fixedSource = @"
using System.Collections.Generic;
namespace TestApp
{
    public class MyClass
    {
        public void Process(SortedList<string, int> items, string key, int value)
        {
            /* [DSA017 / CySec + QA + Design]: Use the collection's atomic operation instead of the check-then-act pattern (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA017.md)
             * MITRE, CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition - https://cwe.mitre.org/data/definitions/367.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            {|#0:if (!items.ContainsKey(key))
            {
                items.Add(key, value);
            }|}
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = DSA017Analyzer.DiagnosticId + ReviewCommentCodeFix.EquivalenceKeySuffix;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("SortedList", "indexer assignment [key] = value"));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA017Analyzer, DSA017CodeFixProvider>.Diagnostic(DSA017Analyzer.DiagnosticId)
            .WithLocation(0).WithArguments("SortedList", "indexer assignment [key] = value"));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public void SanitizeBlockComment_NoInternalClosing_Unchanged()
   {
      var input = "/* safe comment body */";
      var result = ReviewCommentCodeFix.SanitizeBlockComment(input);
      Assert.AreEqual(input, result);
   }

   [TestMethod]
   public void SanitizeBlockComment_InternalClosing_Escaped()
   {
      var input = "/* contains */ inside */";
      var result = ReviewCommentCodeFix.SanitizeBlockComment(input);
      Assert.AreEqual("/* contains * / inside */", result);
   }

   [TestMethod]
   public void SanitizeBlockComment_NotBlockComment_Unchanged()
   {
      var input = "// line comment";
      var result = ReviewCommentCodeFix.SanitizeBlockComment(input);
      Assert.AreEqual(input, result);
   }

   [TestMethod]
   public void SanitizeBlockComment_MultipleInternalClosings()
   {
      var input = "/* a */ b */ c */";
      var result = ReviewCommentCodeFix.SanitizeBlockComment(input);
      Assert.AreEqual("/* a * / b * / c */", result);
   }

   [TestMethod]
   public async Task XmlDocComment_Preserved()
   {
      var source = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        /// <summary>Checks the string.</summary>
        public bool Check(string s)
        {
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var fixedSource = @"
using System;
namespace TestApp
{
    public class MyClass
    {
        /// <summary>Checks the string.</summary>
        public bool Check(string s)
        {
            /* [DSA003 / CySec + QA + Code Smell]: Use `String.IsNullOrWhiteSpace` instead of `String.IsNullOrEmpty` (see: https://github.com/DogmaSolutions/Analyzers/blob/main/docs/rules/DSA003.md)
             * MITRE, CWE-20: Improper Input Validation - https://cwe.mitre.org/data/definitions/20.html
             * IEC 62443-3-3: System security requirements and security levels - https://webstore.iec.ch/en/publication/7033
             */
            return {|#0:string.IsNullOrEmpty(s)|};
        }
    }
}";

      var test = new CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Test();
      test.TestCode = source;
      test.FixedCode = fixedSource;
      test.CodeActionEquivalenceKey = Dsa003Key;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.FixedState.ExpectedDiagnostics.Add(
         CSharpCodeFixVerifier<DSA003Analyzer, DSA003CodeFixProvider>.Diagnostic(DSA003Analyzer.DiagnosticId)
            .WithLocation(0));
      test.NumberOfIncrementalIterations = 1;
      test.NumberOfFixAllIterations = 1;
      test.CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllCheck | CodeFixTestBehaviors.FixOne;

      await test.RunAsync().ConfigureAwait(false);
   }
}
