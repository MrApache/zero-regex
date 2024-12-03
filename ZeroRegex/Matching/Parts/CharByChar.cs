using System.Collections.Generic;

namespace ZeroRegex
{
  internal sealed class CharByChar : Rule
  {
    private readonly Range[] _ranges;

    public CharByChar(Range[] ranges)
    {
      _ranges = ranges;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      return false;
    }
  }

  internal sealed class CharByCharBuilder : IRuleBuilder
  {
    private readonly List<Range> _ranges;

    public CharByCharBuilder()
    {
      _ranges = new List<Range>();
    }

    public void Append(char ch)
    {
      _ranges.Add(new Range(ch));
    }

    public Rule Build()
    {
      return new CharByChar(_ranges.ToArray());
    }
  }
}