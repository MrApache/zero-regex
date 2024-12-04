namespace ZeroRegex
{
  internal sealed class Count : Rule
  {
    private readonly Rule _rule;
    private readonly int _min;
    private readonly int _max;

    public Count(Rule rule, int x, int y)
    {
      _rule = rule;
      _min = x;
      _max = y;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int count = 0;
      for (int j = 0; j < _max; j++) {
        if (_rule.Evaluate(ref context)) {
          count++;
        }
        else {
          break;
        }
      }

      return count >= _min && count <= _max;
    }
  }
  
  internal sealed class QuantifierBuilder : IRuleBuilder
  {
    public bool Quantifiable => false;
    public bool IsEmpty => Target.IsEmpty;
    public bool IsGreedy => _max == int.MaxValue;
    public readonly IRuleBuilder Target;
    private readonly int _min;
    private readonly int _max;

    public QuantifierBuilder(int min, int max, IRuleBuilder target)
    {
      _min = min;
      _max = max;
      Target = target;
    }

    public Rule Build()
    {
      return new Count(Target.Build(), _min, _max);
    }

    public ClassBuilder? GetClassBuilder()
    {
      return Target.GetClassBuilder();
    }
  }
}