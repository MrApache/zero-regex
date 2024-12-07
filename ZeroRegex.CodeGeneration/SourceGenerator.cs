using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace ZeroRegex.CodeGeneration
{
  [Generator]
  internal sealed class SourceGenerator : ISourceGenerator
  {
    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
    if (!Debugger.IsAttached)
    {
        Debugger.Launch();
    }
#endif
    }

    public void Execute(GeneratorExecutionContext context)
    {
      new TypeGenerator().Execute(context);
      //List<IGenerator> generators = new List<IGenerator>();
      //#if NET5_0_OR_GREATER
      //generators.Add(new MethodGenerator());
      //#endif
      //generators.Add(new TypeGenerator());

      //foreach (IGenerator generator in generators) {
      //  generator.Execute(context);
      //}
    }
  }
}