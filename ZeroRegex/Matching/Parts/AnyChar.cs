namespace ZeroRegex
{
  internal sealed class AnyChar : RegexNode
  {
    public AnyChar() : base(null, true)
    {
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int pointer = context.Start + context.Length;
      if (pointer >= context.Text.Length)
        return false;
      return context.Text[context.Start + context.Length++] != '\n';
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string code =
        $@"int pointer = {context.StartIntVariable} + {context.LengthIntVariable};
           if(pointer >= {context.TextVariable}.Length)
             return false;
           return {context.TextVariable}[{context.StartIntVariable} + {context.LengthIntVariable}++] != '\n';";

      string name = CreateUniqueMethodName("AnyCharMatch");
      context.MethodDeclarations.Add(name, new Method(name, code));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode Rebuild()
    {
      return this;
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return false;
    }
  */
  }
}