using Microsoft.CodeAnalysis;

namespace ZeroRegex.CodeGeneration
{
  internal interface IGenerator
  {
    public void Execute(GeneratorExecutionContext context);
  }
}