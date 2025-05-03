using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Operations;

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
            context.RegisterOperationAction(AnalyzeMethod, OperationKind.MethodBodyOperation);
        }

        private static void AnalyzeMethod(OperationAnalysisContext context)
        {
            var method = context.Operation.SemanticModel.GetDeclaredSymbol(context.Operation.Syntax) as IMethodSymbol;
            if (method == null)
                return;

            // Count parameter types
            var typeCounts = new Dictionary<ITypeSymbol, int>(SymbolEqualityComparer.Default);
            foreach (var param in method.Parameters)
            {
                if (param.Type == null)
                    continue;
                if (!typeCounts.ContainsKey(param.Type))
                    typeCounts[param.Type] = 0;
                typeCounts[param.Type]++;
            }

            foreach (var kvp in typeCounts)
            {
                if (kvp.Value >= 3)
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        method.Locations[0],
                        method.Name
                    );
                    context.ReportDiagnostic(diagnostic);
                    break;
                }
            }
        }
    }
}