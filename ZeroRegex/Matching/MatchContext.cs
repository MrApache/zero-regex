using System;

namespace ZeroRegex
{
  internal ref struct MatchContext
  {
    public readonly ReadOnlySpan<char> Text;
    public int Start;
    public int Length;

    public MatchContext(ReadOnlySpan<char> input)
    {
      Text = input;
      Length = 0;
      Start = 0;
    }
  }
}