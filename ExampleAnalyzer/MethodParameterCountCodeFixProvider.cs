using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeActions;
using System.Composition;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace ExampleAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, 
        Name = nameof(MethodParameterCountCodeFixProvider)), Shared]
    public class MethodParameterCountCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create(MethodParameterCountAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the method declaration identified by the diagnostic.
            var methodDecl = root.FindToken(diagnosticSpan.Start).Parent
                .AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodDecl == null)
                return;
            if (methodDecl.ParameterList.Parameters.Count <= 2)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Remove the 3rd or later occurrence of the same type",
                    createChangedDocument: c => RemoveExtraParametersAsync(context.Document, methodDecl, c),
                    equivalenceKey: "RemoveExtraParameters"),
                diagnostic);
        }

        private static async Task<Document> RemoveExtraParametersAsync(Document document,
            MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var typeCounts = new Dictionary<ITypeSymbol, int>(SymbolEqualityComparer.Default);
            var filteredParams = new List<ParameterSyntax>();

            foreach (var param in methodDecl.ParameterList.Parameters)
            {
                var type = semanticModel.GetTypeInfo(param.Type, cancellationToken).Type;
                if (type == null || (typeCounts.TryGetValue(type, out int count) && count < 2) || !typeCounts.ContainsKey(type))
                {
                    filteredParams.Add(param);
                }
                typeCounts[type] = typeCounts.TryGetValue(type, out count) ? count + 1 : 1;
            }

            var newParameterList = methodDecl.ParameterList
                .WithParameters(SyntaxFactory.SeparatedList(filteredParams));
            var newMethodDecl = methodDecl.WithParameterList(newParameterList);
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            return document.WithSyntaxRoot(root.ReplaceNode(methodDecl, newMethodDecl));
        }
    }
}
