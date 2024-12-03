namespace ZeroRegex
{
  internal sealed class Count : Rule
  {
    private readonly Rule _rule;
    private readonly int _min;
    private readonly int _max;

    public bool IsGreedy => _max == int.MaxValue;

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

    public override void Exclude(Range[] values)
    {
      _rule.Exclude(values);
    }

    public override Class? GetClass()
    {
      if (_rule is Class rule)
        return rule;
      return null;
    }

    //public override bool Contains(Range[] values)
    //{
    //  return _rule.Contains(values);
    //}
  }
}