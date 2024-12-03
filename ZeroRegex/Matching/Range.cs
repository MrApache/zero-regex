using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ZeroRegex
{
  internal readonly struct Range : IEquatable<Range>, IComparable<Range>, IEnumerable<char>
  {
    public readonly char Start;
    public readonly char End;

    public int Count => End - Start + 1;

    public static Range Full => new Range(char.MinValue, char.MaxValue);

    public Range(char start)
    {
      Start = start;
      End = start;
    }

    public Range(char start, char end)
    {
      if (start < end) {
        Start = start;
        End = end;
      }
      else {
        Start = end;
        End = start;
      }
    }

    public bool IncludedInRange(char value)
    {
      return value >= Start && value <= End;
    }

    public int CompareTo(Range other)
    {
      return Start.CompareTo(other.Start);
    }

    public override bool Equals(object? obj)
    {
      return obj is Range other && Equals(other);
    }

    public bool Equals(Range other)
    {
      return Start == other.Start && End == other.End;
    }

    public override string ToString()
    {
      return Count == 1
        ? Start.ToString()
        : stackalloc char[] { Start, '-', End }.ToString();
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Start, End);
    }

    public Enumerator GetEnumerator()
    {
      return new Enumerator(this);
    }

    IEnumerator<char> IEnumerable<char>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public static bool TryInclude(Range range, char value, [NotNullWhen(true)] out Range? newRange)
    {
      if (value + 1 == range.Start) {
        newRange = new Range(value, range.End);
        return true;
      }

      if (value - 1 == range.End) {
        newRange = new Range(range.Start, value);
        return true;
      }

      if (range.IncludedInRange(value)) {
        newRange = range;
        return true;
      }

      newRange = null;
      return false;
    }

    public static bool TryMerge(Range first, Range second, [NotNullWhen(true)] out Range? result)
    {
      if (first.IncludedInRange(second.Start)
          || second.Start - 1 == first.Start
          || second.Start - 1 == first.End
          || second.Start + 1 == first.End
          || second.Start + 1 == first.Start) {
        char end = first.End < second.End ? second.End : first.End;
        result = new Range(first.Start, end);
        return true;
      }

      result = null;
      return false;
    }

    public struct Enumerator : IEnumerator<char>
    {
      private readonly Range _range;
      private readonly ushort _counter;
      private char _current;

      public char Current => _current;
      object IEnumerator.Current => Current;

      public Enumerator(Range range)
      {
        _range = range;
        _current = range.Start;
        _counter = (ushort)range.Count;
      }

      public bool MoveNext()
      {
        if (_current + 1 < _counter) {
          _current++;
          return true;
        }

        return false;
      }

      public void Dispose()
      {
      }

      public void Reset()
      {
        _current = _range.Start;
      }
    }
  }
}