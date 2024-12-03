namespace ZeroRegex
{
  internal abstract class Rule
  {
    public abstract bool Evaluate(ref MatchContext context);
    public abstract bool Contains(Range[] values);
  }
}