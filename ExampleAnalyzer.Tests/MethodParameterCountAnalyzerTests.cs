using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Xunit;
using ExampleAnalyzer;

public class MethodParameterCountAnalyzerTests
{
    private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<MethodParameterCountAnalyzer, DefaultVerifier>
        {
            TestCode = source
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    [Fact]
    public async Task Method_WithThreeParameters_NoDiagnostic()
    {
        var code = @"
class C
{
    void M(int a, int b, string c) { }
}";
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Method_WithThreeParametersOfSameType_TriggersDiagnostic()
    {
        var code = @"
class C
{
    void M(int a, System.Int32 b, int c) { }
}";
        var expected = new DiagnosticResult("EA0001", DiagnosticSeverity.Warning)
            .WithSpan(4, 10, 4, 11)
            .WithArguments("M");
        await VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Method_WithFourParametersOfSameType_TriggersDiagnostic()
    {
        var code = @"
class C
{
    void M(int a, int b, int c, int d) { }
}";
        var expected = new DiagnosticResult("EA0001", DiagnosticSeverity.Warning)
            .WithSpan(4, 10, 4, 11)
            .WithArguments("M");
        await VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Method_WithNoParameters_NoDiagnostic()
    {
        var code = @"
class C
{
    void M() { }
}";
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task StaticMethod_WithThreeParametersOfSameType_TriggersDiagnostic()
    {
        var code = @"
class C
{
    static void M(int a, int b, int c) { }
}";
        var expected = new DiagnosticResult("EA0001", DiagnosticSeverity.Warning)
            .WithSpan(4, 17, 4, 18)
            .WithArguments("M");
        await VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task StaticMethod_WithDifferentTypes_NoDiagnostic()
    {
        var code = @"
class C
{
    static void M(int a, string b, double c) { }
}";
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Constructor_WithThreeParametersOfSameType_NoDiagnostic()
    {
        var code = @"
class C
{
    public C(int a, int b, int c) { }
}";
        // Constructors are not analyzed
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task LocalFunction_WithThreeParametersOfSameType_NoDiagnostic()
    {
        var code = @"
class C
{
    void M()
    {
        void Local(int a, int b, int c) { }
    }
}";
        // Local functions are not analyzed
        await VerifyAnalyzerAsync(code);
    }
}