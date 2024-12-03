namespace ZeroRegex
{
  internal readonly struct RuleInfo
  {
    public readonly Rule Rule;
    public readonly bool Quantifiable;
    public readonly bool IsGreedy;

    public RuleInfo(Rule rule, bool quantifiable, bool isGreedy)
    {
      Rule = rule;
      Quantifiable = quantifiable;
      IsGreedy = isGreedy;
    }
  }
}