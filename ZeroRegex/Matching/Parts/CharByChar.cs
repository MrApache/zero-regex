using System.Collections.Generic;
using System.Linq;

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
      if (context.Start + context.Length >= context.Text.Length) {
        return false;
      }

      for (int i = 0; i < _ranges.Length; i++) {
        Range range = _ranges[i];
        for (int j = 0; j < range.Count; j++) {
          int pointer = context.Start + context.Length;
          if (pointer >= context.Text.Length) {
            if (i + 1 >= _ranges.Length && j + 1 >= range.Count) {
              return true;
            }

            return false;
          }

          if (range.IncludedInRange(context.Text[pointer])) {
            context.Length++;
            continue;
          }

          return false;
        }
      }

      return true;
    }
  }

  internal sealed class CharByCharBuilder : IRuleBuilder
  {
    private readonly Stack<Range> _ranges;
    public bool Quantifiable => true;
    public bool IsEmpty => _ranges.Count == 0;

    public CharByCharBuilder()
    {
      _ranges = new Stack<Range>();
    }

    public void Push(char ch)
    {
      _ranges.Push(new Range(ch));
    }

    public Range Pop()
    {
      return _ranges.Pop();
    }

    public Rule Build()
    {
      /*
      Range[] result = PatternBuilder.MergeRanges(_ranges.ToList());
      return new CharByChar(result);
    */
      return new CharByChar(_ranges.Reverse().ToArray());
    }

    public ClassBuilder? GetClassBuilder()
    {
      return null;
    }
  }
}