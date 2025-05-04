using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    ExampleAnalyzer.MethodParameterCountAnalyzer,
    ExampleAnalyzer.MethodParameterCountCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace ExampleAnalyzer.Tests
{
    public class MethodParameterCountCodeFixProviderTests
    {
        [Fact]
        public async Task RemovesExtraParameters_AfterThird()
        {
            var testCode = 
@"class C
{
    void M(int a, char q, int b, int c, int d,
    string a1, int e, string a2, int f, string a3, int g) { }
}";
            var fixedCode = 
@"class C
{
    void M(int a, char q, int b, string a1, string a2) { }
}";

            var expected = VerifyCS.Diagnostic()
                .WithSpan(3, 10, 3, 11)
                .WithArguments("M");

            await VerifyCS.VerifyCodeFixAsync(testCode, expected, fixedCode);
        }

        [Theory]
        [InlineData(
            // Three parameters, same type
            @"class C { void M(int a, int b, int c) { } }",
            @"class C { void M(int a, int b) { } }"
        )]
        [InlineData(
            // Four parameters, one type appears three times
            @"class C { void M(int a, int b, int c, string d) { } }",
            @"class C { void M(int a, int b, string d) { } }"
        )]
        [InlineData(
            // Parameters with default values
            @"class C { void M(int a, int b = 1, int c = 2) { } }",
            @"class C { void M(int a, int b = 1) { } }"
        )]
        [InlineData(
            // Parameters with attributes
            @"class C { void M([Test] int a, int b, int c) { } } class TestAttribute : System.Attribute { }",
            @"class C { void M([Test] int a, int b) { } } class TestAttribute : System.Attribute { }"
        )]
        [InlineData(
            // Mixed types, some types appear more than twice
            @"class C { void M(int a, string b, int c, string d, int e, string f) { } }",
            @"class C { void M(int a, string b, int c, string d) { } }"
        )]
        public async Task CodeFix_Removes_Extra_Parameters_WithDiagnostic(string testCode, string fixedCode)
        {
            var expected = VerifyCS.Diagnostic().WithSpan(1, 16, 1, 17).WithArguments("M");
            await VerifyCS.VerifyCodeFixAsync(testCode, expected, fixedCode);
        }

        [Theory]
        [InlineData(
            // No parameters
            @"class C { void M() { } }"
        )]
        [InlineData(
            // One parameter
            @"class C { void M(int a) { } }"
        )]
        [InlineData(
            // Two parameters, same type
            @"class C { void M(int a, int b) { } }"
        )]
        [InlineData(
            // Four parameters, two types, no type appears more than twice
            @"class C { void M(int a, string b, int c, string d) { } }"
        )]
        public async Task NoDiagnostic_When_ParameterCountIsAcceptable(string testCode)
        {
            await VerifyCS.VerifyAnalyzerAsync(testCode);
        }
    }
}
