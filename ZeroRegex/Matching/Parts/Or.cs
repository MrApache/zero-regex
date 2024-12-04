namespace ZeroRegex
{
  internal sealed class Or : Rule
  {
    private readonly Rule _left;
    private readonly Rule _right;

    public Or(Rule left, Rule right)
    {
      _left = left;
      _right = right;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int start = context.Start;
      int length = context.Length;
      bool leftResult = _left.Evaluate(ref context);
      if (!leftResult) {
        context.Start = start;
        context.Length = length;
        return _right.Evaluate(ref context);
      }

      return true;
    }
  }

  internal sealed class OrBuilder : IRuleBuilder
  {
    public bool Quantifiable => true;
    public bool IsEmpty => false; //TODO

    private readonly IRuleBuilder _left;
    private readonly IRuleBuilder _right;

    public OrBuilder(IRuleBuilder left, IRuleBuilder right)
    {
      _left = left;
      _right = right;
    }

    public Rule Build()
    {
      Rule left = _left.Build();
      Rule right = _right.Build();
      return new Or(left, right);
    }

    public ClassBuilder? GetClassBuilder()
    {
      return null;
    }
  }
}
