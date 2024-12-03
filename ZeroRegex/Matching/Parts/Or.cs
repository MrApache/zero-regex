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
    public void SetLeftRule(Rule rule)
    {

    }

    public Rule Build()
    {
    }
  }
}
