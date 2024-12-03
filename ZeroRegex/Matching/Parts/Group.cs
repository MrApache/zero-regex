namespace ZeroRegex
{
  internal sealed class Group : Rule
  {
    private readonly Rule[] _rules;

    public Group(Rule[] parts)
    {
      _rules = parts;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      foreach (Rule part in _rules) {
        bool status = part.Evaluate(ref context);
        if (!status) return false;
      }

      return true;
    }

    public override void Exclude(Range[] values)
    {
    }

    //public override bool Contains(Range[] values)
    //{
    //  return _rules[^1].Contains(values);
    //}

    public override Class? GetClass()
    {
      return null;
    }
  }
}
