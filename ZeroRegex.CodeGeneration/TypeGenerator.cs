using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZeroRegex.CodeGeneration
{
  internal sealed class TypeGenerator : IGenerator
  {
    private const string Attribute = "CompileRegex";
    public void Execute(GeneratorExecutionContext context)
    {
      List<GeneratedType> types = FindDeclarationsWithAttribute(context.Compilation);
      foreach (GeneratedType generatedType in types) {
        GeneratorContext codeGenContext = new GeneratorContext("input","start", "length");
        PatternBuilder.Build(generatedType.Pattern).GenerateCode(codeGenContext);
        string code = GenerateCode(generatedType.Type, codeGenContext.MethodDeclarations, codeGenContext.InvokationList, generatedType.IsStatic);
        SyntaxNode node = CSharpSyntaxTree.ParseText(code).GetRoot().NormalizeWhitespace();
        context.AddSource($"{generatedType.Type.Identifier}.g.cs", node.ToFullString());
      }
    }

    private static string CreateMethodInvokation(string name)
    {
return $@"start = matchStart + matchLength;
length = 0;
if(!{name}(ref start, ref length, ref input)) {{
  return new Match(start, length, false);
}}
matchLength = start + length - matchStart;
//Console.WriteLine($""{{start}}, {{length}}, {name}"");";
    }

    private static string GenerateCode(TypeDeclarationSyntax typeDeclarationSyntax, Dictionary<string, Method> methods, List<string> invokationList, bool isStatic)
    {
      string firstInvokationMethod = string.Empty;
      string invokationListCode = string.Empty;
      string methodsDecls = string.Empty;

      if (methods.Count > 0) {
        string first = invokationList[0];
        invokationList.RemoveAt(0);
        firstInvokationMethod =
$@"int start = 0;
int length = 0;
if(!{first}(ref start, ref matchLength, ref input)) {{
  return new Match(0, 0, false);
}}
matchStart = start;
length = start - matchStart;";
      }

      foreach (string methodName in invokationList) {
        invokationListCode += CreateMethodInvokation(methodName) + '\n';
      }

      foreach (string name in methods.Keys) {
        methodsDecls += '\n';
        methodsDecls +=
@$"private {GetStaticStringKeyword(isStatic)} bool {name}(ref int start, ref int length, ref ReadOnlySpan<char> input)
{{
  {methods[name].Content}
}}";
      }

      const string usings = "using ZeroRegex;\n" +
                            "using System.CodeDom.Compiler;";
      string code = string.Format(
@"
[GeneratedCode(""ZeroRegex.CodeGenerator"", ""1.0.0"")]
{0} {1} {2}
{{
  public {6} Match Match(ReadOnlySpan<char> input)
  {{
    int matchStart = -1;
    int matchLength = 0;
    {3}
    {4}

    return new Match(matchStart, matchLength, true);
  }}
  {5}
}}",
        typeDeclarationSyntax.Modifiers,
        typeDeclarationSyntax.Keyword,
        typeDeclarationSyntax.Identifier,
        firstInvokationMethod,
        invokationListCode,
        methodsDecls,
        GetStaticStringKeyword(isStatic));

      string? namesp = GetNamespace(typeDeclarationSyntax);
      if (namesp != null) {
        code = $@"
using System;
namespace {namesp}
{{
  {code}
}}";
      }
      else {
        code = "using System;" +
               $"{code}";
      }

      return usings + code;
    }

    private static string GetStaticStringKeyword(bool isStatic)
    {
      return isStatic ? "static" : string.Empty;
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

    private static List<TypeDeclarationSyntax> FindAllClassAndStructDeclarations(Compilation compilation)
    {
      List<TypeDeclarationSyntax> typeDeclarations = new List<TypeDeclarationSyntax>();

      foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
      {
        SyntaxNode root = syntaxTree.GetRoot();

        IEnumerable<TypeDeclarationSyntax> types = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        typeDeclarations.AddRange(types);
        types = root.DescendantNodes().OfType<StructDeclarationSyntax>();
        typeDeclarations.AddRange(types);
      }

      return typeDeclarations;
    }

    private static List<GeneratedType> FindDeclarationsWithAttribute(Compilation compilation)
    {
      List<GeneratedType> typesWithAttribute = new List<GeneratedType>();
      List<TypeDeclarationSyntax> types = FindAllClassAndStructDeclarations(compilation);
      foreach (TypeDeclarationSyntax type in types.Where(t => t.Modifiers.Any(SyntaxKind.PartialKeyword))) {
        SemanticModel semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
        foreach (AttributeListSyntax attributeList in type.AttributeLists) {
          AttributeSyntax? attributeSyntax = attributeList.Attributes.FirstOrDefault(attrb => attrb.Name.ToString().Equals(Attribute));
          if (attributeSyntax != null) {
            string pattern = ReadAttributeArgument<string>(attributeSyntax, semanticModel, 0);
            bool isStatic = ReadAttributeArgument<bool>(attributeSyntax, semanticModel, 1);
            GeneratedType generatedType = new GeneratedType(type, pattern, isStatic);
            typesWithAttribute.Add(generatedType);
          }
        }
      }

      return typesWithAttribute;
    }

    private static TReturn ReadAttributeArgument<TReturn>(AttributeSyntax attributeSyntax, SemanticModel semanticModel, int index)
    {
      SeparatedSyntaxList<AttributeArgumentSyntax> attributeArguments = attributeSyntax.ArgumentList!.Arguments;
      if (attributeArguments.Count <= index)
        return default!;
      AttributeArgumentSyntax attributeArgumentSyntax = attributeArguments[index];
      Optional<object?> constantValue = semanticModel.GetConstantValue(attributeArgumentSyntax.Expression);
      return (TReturn)constantValue.Value!;
    }

    private readonly struct GeneratedType
    {
      public readonly TypeDeclarationSyntax Type;
      public readonly string Pattern;
      public readonly bool IsStatic;

      public GeneratedType(TypeDeclarationSyntax type, string pattern, bool isStatic)
      {
        Type = type;
        Pattern = pattern;
        IsStatic = isStatic;
      }
    }
  }
}