using System.Collections.Generic;

namespace ZeroRegex
{
  internal sealed class Class : Rule
  {
    private readonly Range[] _ranges;

    public Class(Range[] ranges)
    {
      _ranges = ranges;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      bool status = false;
      if (context.Length + context.Start < context.Text.Length) {
        foreach (Range range in _ranges) {
          if (range.IncludedInRange(context.Text[context.Length + context.Start])) {
            status = true;
            break;
          }
        }
      }

      if (status) {
        context.Length++;
      }

      return status;
    }

  }

  internal sealed class ClassBuilder : IRuleBuilder
  {
    private readonly List<Range> _ranges;

    public ClassBuilder()
    {
      _ranges = new List<Range>();
    }

    public void Exclude(Range[] values)
    {
      List<Range> list = new List<Range>();
      foreach (Range range in _ranges) {
        list.AddRange(PatternBuilder.ExcludeRanges(range, values));
      }

      _ranges.Clear();
      _ranges.AddRange(PatternBuilder.MergeRanges(list));
    }

    public Rule Build()
    {
      return new Class(_ranges.ToArray());
    }
  }
}
