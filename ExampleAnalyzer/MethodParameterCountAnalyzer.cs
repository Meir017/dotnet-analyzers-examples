using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace ExampleAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParameterCountAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "EA0001",
            title: "Method has too many parameters",
            messageFormat: "Method '{0}' has more than 3 parameters",
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Methods should not have more than 3 parameters."
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDecl = (MethodDeclarationSyntax)context.Node;
            if (methodDecl.ParameterList.Parameters.Count > 3)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDecl.Identifier.GetLocation(),
                    methodDecl.Identifier.Text
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}