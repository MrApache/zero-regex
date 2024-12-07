using System.Collections.Generic;
using System.Text;
using ZeroRegex.Utils;

namespace ZeroRegex
{
  internal sealed class Class : RegexNode
  {
    private readonly List<Range> _ranges;

    public Class(Range[] ranges) : base(null, true)
    {
      _ranges = new List<Range>(ranges);
    }

    private static (PoolBuffer<Range> buffer, int items) ExcludeRanges(Range target, List<Range> ranges)
    {
      PoolBuffer<Range> result = new PoolBuffer<Range>(ranges.Count * 2);
      result[0] = target;
      int resultPointer = 1;

      using PoolBuffer<Range> temp = new PoolBuffer<Range>(ranges.Count * 2);
      int pointer = 0;

      foreach (Range range in ranges) {
        for (int i = 0; i < resultPointer; i++) {
          Range current = result[i];
          if (current.End < range.Start || current.Start > range.End) {
            temp[pointer++] = current;
          }
          else {
            if (current.Start < range.Start) {
              temp[pointer++] = new Range(current.Start, (char)(range.Start - 1));
            }

            if (current.End > range.End) {
              temp[pointer++] = new Range((char)(range.End + 1), current.End);
            }
          }
        }

        for (int i = resultPointer; i < pointer + resultPointer; i++) {
          result[i] = temp[i];
        }

        resultPointer = pointer;
        pointer = 0;
      }
      result.Sort();
      return (result, resultPointer);
    }

    private static int MergeRanges(PoolBuffer<Range> ranges, int itemsCount)
    {
      if (ranges.Count < 2)
        return itemsCount;

      int freeIndex = -1;
      int removedItems = 0;
      for (int i = 0; i <= itemsCount - 1; i++) {
        Range first = ranges[i];
        Range second = ranges[i + 1];

        if (Range.TryMerge(first, second, out Range? result)) {
          if (freeIndex != -1) {
            ranges[freeIndex] = result.Value;
          }
          else {
            ranges[i] = result.Value;
          }
          freeIndex = i + 1;
          removedItems++;
        }
        else {
          i++;
        }
      }

      return itemsCount - removedItems;
    }

    private void Exclude(List<Range> values)
    {
      using PoolBuffer<Range> pool = new PoolBuffer<Range>(_ranges.Count * 2);
      int pointer = 0;
      foreach (Range range in _ranges) {
        (PoolBuffer<Range> buffer, int items) = ExcludeRanges(range, values);
        items = MergeRanges(buffer, items);
        for (int i = 0; i < items; i++) {
          Range result = buffer[i];
          pool[pointer++] = result;
        }
      }

      MergeRanges(pool, pointer);
      _ranges.Clear();
      _ranges.AddRange(pool);
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int currentIndex = context.Start + context.Length;
      if (currentIndex >= context.Text.Length)
        return false;

      char currentChar = context.Text[currentIndex];
      foreach (Range range in _ranges) {
        if (range.IncludedInRange(currentChar)) {
          context.Length++;
          return true;
        }
      }

      return false;
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      StringBuilder foreachLoop = new StringBuilder();
      foreach (Range range in _ranges) {
        foreachLoop.Append(
$@"if(currentChar >= '{range.Start}' && currentChar <= '{range.End}') {{
  {context.LengthIntVariable}++;
  return true;
}}");
        foreachLoop.AppendLine();
      }

      string code =
$@"int currentIndex = {context.StartIntVariable} + {context.LengthIntVariable};
if (currentIndex >= {context.TextVariable}.Length)
  return false;
char currentChar = {context.TextVariable}[currentIndex];
{foreachLoop}
return false;     
";

      string name = CreateUniqueMethodName("FindMatchInClass");
      context.MethodDeclarations.Add(name, new Method(name, code));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode? Rebuild()
    {
      if (_ranges.Count == 0)
        return null;

      if (_ranges.Count == 1 && _ranges[0].Count == 1)
        return new Char(_ranges[0].Start);

      return this;
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return a is Class;
    }

    public override RegexNode Merge(RegexNode a)
    {
      Class cl = (Class)a;
      Exclude(cl._ranges);
      return this;
    }
  */
  }
}
