using System;
using System.Text;

namespace ZeroRegex
{
  public sealed class Pattern
  {
    private readonly RegexNode[] _parts;

    internal Pattern(RegexNode[] parts)
    {
      _parts = parts;
    }

    public Match Match(string input)
    {
      int start = -1;
      int length = 0;
      ReadOnlySpan<char> span = input;
      for (int offset = 0; offset < input.Length; offset++) {
        foreach (RegexNode rule in _parts) {
          MatchContext context = new MatchContext(span);
          context.Start = start == -1 ? offset : start + length;
          bool status = rule.Evaluate(ref context);

          if (!status) break;

          if (start == -1) {
            start = context.Start;
            //if ((_anchors & Anchor.StartOfLine) != 0 && start != 0) {
            //  return new Match(0, 0, false);
            //}
          }

          length = context.Start + context.Length - start;
        }
      }

      if (start == -1) {
        return new Match(0, 0, false);
      }

      //if ((_anchors & Anchor.EndOfLine) != 0 && start + length != input.Length) {
      //  return new Match(0, 0, false);
      //}

      return new Match(start, length, true);
    }

    internal string GenerateCode(GeneratorContext context)
    {
      StringBuilder sb = new StringBuilder();
      foreach (RegexNode rule in _parts) {
        sb.Append(rule.GenerateMethod(context));
        sb.AppendLine();
      }

      return sb.ToString();
    }
  }
}