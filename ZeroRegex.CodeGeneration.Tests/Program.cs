using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ZeroRegex.CodeGeneration.Tests;

internal static class Program
{
  private const string _code =
    @"
using ZeroRegex;
[CompileRegex(""Hello world"", true)]
public partial class SomeClass
{{
  public void SomeMethod()
  {{
    
  }}
}}";

  public static void Main()
  {
    Compilation compilation = CreateCompilation(_code);
    SourceGenerator generator = new SourceGenerator();
    GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
    driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
  }

  private static Compilation CreateCompilation(string source)
    => CSharpCompilation.Create("compilation",
      [CSharpSyntaxTree.ParseText(source)],
      [MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)],
      new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}
