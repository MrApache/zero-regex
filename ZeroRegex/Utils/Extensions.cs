using System;

namespace ZeroRegex.Utils
{
  internal static class Extensions
  {
    public static int IndexOf(this ReadOnlySpan<char> span, char ch, int position)
    {
      span = span[position..];
      for (int i = 0; i < span.Length; i++) {
        if (span[i] == ch)
          return i;
      }

      return -1;
    }
  }
}