using System;

namespace ZeroRegex
{
  public sealed class Matcher
  {
    private readonly Rule[] _parts;
    private readonly Anchor _anchors;

    public Matcher(string pattern)
    {
      //Regex
      //_pattern = PatternBuilder.Build(pattern);
    }

    public Match Match(string input)
    {
      ThrowIfHasErrors(input, 0, input.Length);
      return Match(input.AsSpan());
    }

    public Match Match(string input, int start)
    {
      int length = input.Length - start;
      ThrowIfHasErrors(input, start, length);
      return Match(input.AsSpan(start, length));
    }

    public Match Match(string input, int start, int length)
    {
      ThrowIfHasErrors(input, start, length);
      return Match(input.AsSpan(start, length));
    }

    public Match Match(ReadOnlySpan<char> input, int start)
    {
      int length = input.Length - start;
      ThrowIfHasErrors(input, start, length);
      return Match(input.Slice(start, length));
    }

    public Match Match(ReadOnlySpan<char> input, int start, int length)
    {
      ThrowIfHasErrors(input, start, length);
      return Match(input.Slice(start, length));
    }

    public Match Match(ReadOnlySpan<char> input)
    {
      int start = -1;
      int length = 0;
      for (int offset = 0; offset < input.Length; offset++) {
        foreach (Rule rule in _parts) {
          MatchContext context = new MatchContext(input);
          context.Start = start == -1 ? offset : start + length;
          bool status = rule.Evaluate(ref context);

          if (!status) break;

          if (start == -1) {
            start = context.Start;
            if ((_anchors & Anchor.StartOfLine) != 0 && start != 0) {
              return new Match(0, 0, false);
            }
          }

          length = context.Start + context.Length - start;
        }
      }

      if (start == -1) {
        return new Match(0, 0, false);
      }

      if ((_anchors & Anchor.EndOfLine) != 0 && start + length != input.Length) {
        return new Match(0, 0, false);
      }

      return new Match(start, length, true);
    }

    private static void ThrowIfHasErrors(string input, int start, int length)
    {
      if (string.IsNullOrEmpty(input))
        throw new Exception("Empty string");
      if (start < 0)
        throw new ArgumentOutOfRangeException(nameof(start));
      if (length > input.Length || length < 0)
        throw new ArgumentOutOfRangeException(nameof(length));
    }

    private static void ThrowIfHasErrors(ReadOnlySpan<char> input, int start, int length)
    {
      if (input.IsEmpty)
        throw new Exception("String is empty");
      if (start < 0)
        throw new ArgumentOutOfRangeException(nameof(start));
      if (length > input.Length || length < 0)
        throw new ArgumentOutOfRangeException(nameof(length));
    }
  }
}