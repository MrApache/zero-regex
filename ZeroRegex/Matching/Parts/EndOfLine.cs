namespace ZeroRegex
{
  internal sealed class EndOfLine : RegexNode
  {
    public EndOfLine() : base(null, false)
    {
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int position = context.Start + context.Length;
      return position == context.Length || context.Text[position] == '\n';
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string code = $@"
int position = {context.StartIntVariable} + {context.LengthIntVariable};
return position == {context.LengthIntVariable} || {context.TextVariable}[position] == '\n';";

      string name = CreateUniqueMethodName("IsEndOfLine");
      context.RegisterMethod(new Method(name, code));
      return name;
    }

    public override RegexNode Rebuild()
    {
      return this;
    }
  }
}