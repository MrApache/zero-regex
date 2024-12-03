namespace ZeroRegex
{
  internal abstract class Rule
  {
    public abstract bool Evaluate(ref MatchContext context);
    //public abstract void Exclude(Range[] values);
    //public abstract Class? GetClass();
  }
}