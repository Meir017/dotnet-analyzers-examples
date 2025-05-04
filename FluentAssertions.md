
Real world example of writing Roslyn Analyzers.

The `DiagnosticAnalyzer`:

**Performance is important, so we need to be careful with the code we write.**

1. Setup: [FluentAssertionsAnalyzer.cs](vscode://file/C:/git/fluentassertions/fluentassertions.analyzers/src/FluentAssertions.Analyzers/Tips/FluentAssertionsAnalyzer.cs:23)

    1.1. Return early if possible

    1.2. Create a class to hold references to used types [FluentAssertionsMetadata.cs](vscode://file/C:/git/fluentassertions/fluentassertions.analyzers/src/FluentAssertions.Analyzers/Tips/FluentAssertionsAnalyzer.Utils.cs)

2. Analyzing IOperation: 

    2.1. Exit early conditions [FluentAssertionsAnalyzer.cs](vscode://file/C:/git/fluentassertions/fluentassertions.analyzers/src/FluentAssertions.Analyzers/Tips/FluentAssertionsAnalyzer.cs:41:37)

    2.2. Switch Case and handle multiple cases [FluentAssertionsAnalyzer.cs](vscode://file/C:/git/fluentassertions/fluentassertions.analyzers/src/FluentAssertions.Analyzers/Tips/FluentAssertionsAnalyzer.cs:63)

The `CodeFixProvider`:

**Performance is less important, so we don't need to be as careful with the code we write.**

1. leverage the `DocumentEditor` to make changes to the document [FluentAssertionsCodeFixProvider.cs](vscode://file/C:/git/fluentassertions/fluentassertions.analyzers/src/FluentAssertions.Analyzers/Tips/Editing/CreateEquivalencyAssertionOptionsLambda.cs:8)

2. use the `SyntaxGenerator` in the `DocumentEditor` to create the new code when it's usful.