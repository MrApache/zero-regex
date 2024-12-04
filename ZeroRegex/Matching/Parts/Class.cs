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
    public readonly List<Range> Ranges;
    public bool Quantifiable => true;
    public bool IsEmpty => Ranges.Count == 0;

    public ClassBuilder(params Range[] ranges)
    {
      Ranges = new List<Range>(ranges);
    }

    public void Exclude(Range[] values)
    {
      List<Range> list = new List<Range>();
      foreach (Range range in Ranges) {
        list.AddRange(PatternBuilder.ExcludeRanges(range, values));
      }

      Ranges.Clear();
      Ranges.AddRange(PatternBuilder.MergeRanges(list));
    }

    public Rule Build()
    {
      return new Class(Ranges.ToArray());
    }

    public ClassBuilder GetClassBuilder()
    {
      return this;
    }
  }
}
