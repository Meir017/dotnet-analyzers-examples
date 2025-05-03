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
    void M(int a, int b, int c) { }
}";
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Method_WithFourParameters_TriggersDiagnostic()
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
    public async Task StaticMethod_WithFourParameters_TriggersDiagnostic()
    {
        var code = @"
class C
{
    static void M(int a, int b, int c, int d) { }
}";
        var expected = new DiagnosticResult("EA0001", DiagnosticSeverity.Warning)
            .WithSpan(4, 17, 4, 18)
            .WithArguments("M");
        await VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Constructor_WithFourParameters_NoDiagnostic()
    {
        var code = @"
class C
{
    public C(int a, int b, int c, int d) { }
}";
        await VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task LocalFunction_WithFourParameters_NoDiagnostic()
    {
        var code = @"
class C
{
    void M()
    {
        void Local(int a, int b, int c, int d) { }
    }
}";
        await VerifyAnalyzerAsync(code);
    }
}