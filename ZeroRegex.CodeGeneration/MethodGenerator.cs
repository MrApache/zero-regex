using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroRegex.CodeGeneration
{
  internal sealed class MethodGenerator : IGenerator
  {
    public void Execute(GeneratorExecutionContext context)
    {
      IEnumerable<IMethodSymbol> methods = FindMethodsWithAttribute(context.Compilation, nameof(CompileRegexAttribute));
      IEnumerable<IMethodSymbol> regexMethods =
        methods.Where(method => method.IsPartialDefinition
                                && method.ReturnType.Name == nameof(Pattern)
                                && !method.IsImplicitlyDeclared);
      IMethodSymbol methodSymbol = regexMethods.ElementAt(0);
      MethodDeclarationSyntax method = (MethodDeclarationSyntax)methodSymbol.DeclaringSyntaxReferences[0].GetSyntax();
      INamedTypeSymbol type = methodSymbol.ContainingType;
      SyntaxReference syntaxReference = type.DeclaringSyntaxReferences[0];
      SyntaxNode syntaxNode = syntaxReference.GetSyntax();
      if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax) {
        GenerateCode(method, typeDeclarationSyntax, context);
      }
      else {
        throw new Exception($"IDK what is this {syntaxNode.GetType().Name}");
      }
    }

    private static void GenerateCode(MethodDeclarationSyntax method, TypeDeclarationSyntax typeDeclarationSyntax, GeneratorExecutionContext context)
    {
      const string usings = "using ZeroRegex;\n";
      string code = string.Format(
@"{0} {1} {2}
{{
  {3} {4} {5}{6}
  {{
    {7} 
  }}
}}",
        typeDeclarationSyntax.Modifiers,
        typeDeclarationSyntax.Keyword,
        typeDeclarationSyntax.Identifier,
        method.Modifiers,
        method.ReturnType,
        method.Identifier,
        method.ParameterList,
        "return null!;");

      string? namesp = GetNamespace(typeDeclarationSyntax);
      if (namesp != null) {
        code = $@"namespace {namesp}
{{
  {code}
}}";
      }

      context.AddSource("Type.g.cs", usings + code);
    }

    private static string? GetNamespace(TypeDeclarationSyntax typeDeclaration)
    {
      SyntaxNode? current = typeDeclaration.Parent;

      while (current != null) {
        switch (current) {
          case NamespaceDeclarationSyntax namespaceDeclaration:
            return namespaceDeclaration.Name.ToString();
          case FileScopedNamespaceDeclarationSyntax fileScopedNamespace:
            return fileScopedNamespace.Name.ToString();
          default:
            current = current.Parent;
            break;
        }
      }

      return null;
    }

    private static IEnumerable<IMethodSymbol> FindMethodsWithAttribute(Compilation compilation, string attributeName)
    {
      List<IMethodSymbol> methodsWithAttribute = new List<IMethodSymbol>();

      foreach (SyntaxTree? syntaxTree in compilation.SyntaxTrees) {
        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
        SyntaxNode root = syntaxTree.GetRoot();

        IEnumerable<MethodDeclarationSyntax> methodDeclarations =
          root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations) {
          IMethodSymbol? methodSymbol = (IMethodSymbol?)semanticModel.GetDeclaredSymbol(methodDeclaration);

          if (methodSymbol != null) {
            bool hasAttribute = methodSymbol.GetAttributes().Any(attr =>
              attr.AttributeClass != null &&
              attr.AttributeClass.Name == attributeName ||
              attr.AttributeClass!.ToDisplayString() == attributeName);

            if (hasAttribute) {
              methodsWithAttribute.Add(methodSymbol);
            }
          }
        }
      }

      return methodsWithAttribute;
    }
  }
}