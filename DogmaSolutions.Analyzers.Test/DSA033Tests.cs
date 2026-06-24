using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable All

namespace DogmaSolutions.Analyzers.Test;

[TestClass]
public class DSA033Tests
{
   #region Matched (file exceeds threshold)

   [TestMethod]
   public async Task Flags_FileExceedingDefaultThreshold()
   {
      var lines = new string[502];
      lines[0] = "namespace TestApp {";
      lines[1] = "    public class MyClass {";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "    }";
      lines[501] = "}";
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 20)
            .WithArguments("Test0.cs", 502, 500));

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_FileExceedingCustomThreshold()
   {
      var source = @"
namespace TestApp
{
    public class ClassA
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 5
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 1)
            .WithArguments("Test0.cs", 12, 5));

      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion

   #region Not matched (file under threshold)

   [TestMethod]
   public async Task DoesNotFlag_FileUnderDefaultThreshold()
   {
      var source = @"
namespace TestApp
{
    public class MyClass
    {
        public int Value { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_FileExactlyAtThreshold()
   {
      var lines = new string[500];
      lines[0] = "namespace TestApp {";
      lines[1] = "    public class MyClass {";
      for (int i = 2; i < 498; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[498] = "    }";
      lines[499] = "}";
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_FileUnderCustomThreshold()
   {
      var source = @"
namespace TestApp
{
    public class ClassA
    {
        public int A { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 100
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_InvalidThresholdUsesDefault()
   {
      var source = @"
namespace TestApp
{
    public class MyClass
    {
        public int Value { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = -1
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_NonNumericThresholdUsesDefault()
   {
      var source = @"
namespace TestApp
{
    public class MyClass
    {
        public int Value { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = abc
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_ZeroThresholdUsesDefault()
   {
      var source = @"
namespace TestApp
{
    public class MyClass
    {
        public int Value { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 0
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion

   #region File pattern exclusions

   [TestMethod]
   public async Task DoesNotFlag_ExcludedFilePattern()
   {
      var lines = new string[502];
      lines[0] = "namespace TestApp {";
      lines[1] = "    public class MyWidget {";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "    }";
      lines[501] = "}";
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("MyForm.Designer.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_CustomExcludedFilePattern()
   {
      var source = @"
namespace TestApp
{
    public class ClassA
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("MyReport.auto.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_file_patterns = *.auto.cs
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_MultipleCommaSeparatedFilePatterns()
   {
      var source = @"
namespace TestApp
{
    public class MyWidget
    {
        public int Alpha { get; set; }
        public int Beta { get; set; }
        public int Gamma { get; set; }
        public int Delta { get; set; }
        public int Epsilon { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("MyReport.special.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_file_patterns = *.auto.cs, *.special.cs, *.tmp.cs
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_CustomFilePatterns_OverrideDefaults()
   {
      // MyOutput.special.cs would NOT match any default pattern,
      // but if the user sets custom patterns to *.auto.cs, the defaults
      // are replaced and *.special.cs is also not excluded.
      // This proves custom patterns replace rather than extend the defaults.
      var lines = new string[502];
      lines[0] = "namespace TestApp {";
      lines[1] = "    public class MyOutputWidget {";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "    }";
      lines[501] = "}";
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("MyOutput.special.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_file_patterns = *.auto.cs
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan("MyOutput.special.cs", 1, 1, 1, 20)
            .WithArguments("MyOutput.special.cs", 502, 500));
      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion

   #region Base type exclusions

   [TestMethod]
   public async Task DoesNotFlag_ExcludedBaseType()
   {
      var lines = new string[502];
      lines[0] = "public class MyHandler : System.Exception {";
      lines[1] = "    public MyHandler() : base() { }";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "}";
      lines[501] = string.Empty;
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_NonExcludedBaseType()
   {
      var lines = new string[502];
      lines[0] = "public class MyHandler : System.Exception {";
      lines[1] = "    public MyHandler() : base() { }";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "}";
      lines[501] = string.Empty;
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_base_types = System.NotImplementedException
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 44)
            .WithArguments("Test0.cs", 502, 500));

      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_ExcludedBaseTypeInherited()
   {
      var baseSource = @"public class MiddleBase : System.Exception { }";

      var lines = new string[502];
      lines[0] = "public class MyHandler : MiddleBase {";
      lines[1] = "    public MyHandler() : base() { }";
      for (int i = 2; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "}";
      lines[501] = string.Empty;
      var derivedSource = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("/0/Base.cs", baseSource));
      test.TestState.Sources.Add(("/0/Test0.cs", derivedSource));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_DefaultBaseTypeExclusion()
   {
      var baseSource = @"namespace Microsoft.EntityFrameworkCore.Migrations { public class Migration { } }";

      var lines = new string[502];
      lines[0] = "public class MyDataUpdate : Microsoft.EntityFrameworkCore.Migrations.Migration {";
      for (int i = 1; i < 500; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[500] = "}";
      lines[501] = string.Empty;
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("/0/FakeEf.cs", baseSource));
      test.TestState.Sources.Add(("/0/Test0.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_MultiTypeFile_OneInheritsExcluded()
   {
      var lines = new string[502];
      lines[0] = "public class MyErrorHandler : System.Exception {";
      lines[1] = "    public MyErrorHandler() : base() { }";
      for (int i = 2; i < 250; i++)
         lines[i] = $"        public int P{i} {{ get; set; }}";
      lines[250] = "}";
      lines[251] = "public class MyPlainWidget {";
      for (int i = 252; i < 500; i++)
         lines[i] = $"        public int Q{i} {{ get; set; }}";
      lines[500] = "}";
      lines[501] = string.Empty;
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_NoTypesInFile()
   {
      var lines = new string[502];
      lines[0] = "using System;";
      for (int i = 1; i < 501; i++)
         lines[i] = $"// Line {i}";
      lines[501] = string.Empty;
      var source = string.Join("\n", lines);

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 14)
            .WithArguments("Test0.cs", 502, 500));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_MultipleCommaSeparatedBaseTypes()
   {
      var source = @"
public class MyFaultHandler : System.Exception
{
    public MyFaultHandler() : base() { }
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Gamma { get; set; }
    public int Delta { get; set; }
    public int Epsilon { get; set; }
    public int Zeta { get; set; }
    public int Eta { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_base_types = System.IO.IOException, System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_InterfaceType_BaseTypeExclusionConfigured()
   {
      var source = @"
public interface IMyContract
{
    int Alpha { get; set; }
    int Beta { get; set; }
    int Gamma { get; set; }
    int Delta { get; set; }
    int Epsilon { get; set; }
    int Zeta { get; set; }
    int Eta { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 1)
            .WithArguments("Test0.cs", 11, 10));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_StructType_BaseTypeExclusionConfigured()
   {
      var source = @"
public struct MyDataPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int W { get; set; }
    public int V { get; set; }
    public int U { get; set; }
    public int T { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 1)
            .WithArguments("Test0.cs", 11, 10));
      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion

   #region Interaction exclusions

   [TestMethod]
   public async Task DoesNotFlag_BothExclusionsConfigured_FilePatternMatches()
   {
      var source = @"
public class MyReportGenerator
{
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Gamma { get; set; }
    public int Delta { get; set; }
    public int Epsilon { get; set; }
    public int Zeta { get; set; }
    public int Eta { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestState.Sources.Add(("MyReport.special.cs", source));
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_file_patterns = *.special.cs
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_BothExclusionsConfigured_BaseTypeMatches()
   {
      var source = @"
public class MyFaultHandler : System.Exception
{
    public MyFaultHandler() : base() { }
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Gamma { get; set; }
    public int Delta { get; set; }
    public int Epsilon { get; set; }
    public int Zeta { get; set; }
    public int Eta { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_file_patterns = *.auto.cs
dotnet_diagnostic.DSA033.excluded_base_types = System.Exception
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_BothExclusionsConfigured_NeitherMatches()
   {
      var source = @"
public class MyFaultHandler : System.Exception
{
    public MyFaultHandler() : base() { }
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Gamma { get; set; }
    public int Delta { get; set; }
    public int Epsilon { get; set; }
    public int Zeta { get; set; }
    public int Eta { get; set; }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
dotnet_diagnostic.DSA033.excluded_file_patterns = *.auto.cs
dotnet_diagnostic.DSA033.excluded_base_types = System.NotImplementedException
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 1)
            .WithArguments("Test0.cs", 12, 10));
      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion

   #region Blank line counting

   [TestMethod]
   public async Task DoesNotFlag_BlankLinesExcludedByDefault()
   {
      // 15 total lines, 9 non-blank. Threshold = 10. Should not flag (9 <= 10).
      var source = @"namespace TestApp
{
    public class MyWidget
    {
        public int Alpha { get; set; }

        public int Beta { get; set; }

        public int Gamma { get; set; }




    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_BlankLinesCountedWhenEnabled()
   {
      // 15 total lines, threshold = 10. With count_blank_lines = true, 15 > 10 → flags.
      var source = @"namespace TestApp
{
    public class MyWidget
    {
        public int Alpha { get; set; }

        public int Beta { get; set; }

        public int Gamma { get; set; }




    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = true
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 18)
            .WithArguments("Test0.cs", 15, 10));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task Flags_NonBlankLinesExceedThreshold_BlankLinesExcluded()
   {
      // 12 total lines, 11 non-blank, threshold = 10. Default (no blank) → 11 > 10 → flags.
      var source = @"namespace TestApp
{
    public class MyWidget
    {
        public int Alpha { get; set; }
        public int Beta { get; set; }
        public int Gamma { get; set; }
        public int Delta { get; set; }

        public int Epsilon { get; set; }
    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestBehaviors = TestBehaviors.SkipSuppressionCheck;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
"));
      test.ExpectedDiagnostics.Add(
         CSharpAnalyzerVerifier<DSA033Analyzer>.Diagnostic(DSA033Analyzer.DiagnosticId)
            .WithSpan(1, 1, 1, 18)
            .WithArguments("Test0.cs", 11, 10));
      await test.RunAsync().ConfigureAwait(false);
   }

   [TestMethod]
   public async Task DoesNotFlag_InvalidCountBlankLinesUsesDefault()
   {
      // Invalid boolean falls back to default (false). 15 total, 9 non-blank, threshold = 10. No flag.
      var source = @"namespace TestApp
{
    public class MyWidget
    {
        public int Alpha { get; set; }

        public int Beta { get; set; }

        public int Gamma { get; set; }




    }
}";

      var test = new CSharpAnalyzerVerifier<DSA033Analyzer>.Test();
      test.TestCode = source;
      test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
      test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", @"
root = true
[*]
dotnet_diagnostic.DSA033.max_lines = 10
dotnet_diagnostic.DSA033.count_blank_lines = maybe
"));
      await test.RunAsync().ConfigureAwait(false);
   }

   #endregion
}
