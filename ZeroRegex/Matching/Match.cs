using System;
using ZeroRegex.Utils;

namespace ZeroRegex
{
  public readonly struct Match : IEquatable<Match>
  {
    public readonly int Start;
    public readonly int Length;
    public readonly bool Success;

    public Match(int start, int length, bool success)
    {
      Start = start;
      Length = length;
      Success = success;
    }

    public bool Equals(Match other)
    {
      return Start == other.Start
             && Length == other.Length
             && Success == other.Success;
    }

    public override bool Equals(object? obj)
    {
      return obj is Match match
             && Equals(match);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Start, Length, Success);
    }

    public override string ToString()
    {
      if (!Success) {
        return "None";
      }

      const int size = ValueStringBuilder.IntMaxCharsCount * 2 + 1;
      Span<char> temp = stackalloc char[size];
      ValueStringBuilder sb = new ValueStringBuilder(temp);
      sb.Append(Start);
      sb.Append(':');
      sb.Append(Length);
      return temp.ToString();
    }
  }
}