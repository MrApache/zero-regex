using System;
using System.Collections.Generic;
using System.Linq;
using ZeroRegex.Utils;

namespace ZeroRegex
{
  public static class PatternBuilder
  {
    private const int PATTERN_CAPACITY = 8;
    private const int CLASS_CAPACITY = 16;

    private static readonly Range[] _whitespaces;
    private static readonly Range[] _nonWhitespaces;
    private static readonly Range[] _wordCharacters;
    private static readonly Range[] _nonWordCharacters;
    private static readonly Range[] _numbers;
    private static readonly Range[] _nonNumbers;
    private static readonly Range[] _allExceptNewline;

    static PatternBuilder()
    {
      _whitespaces = new[] {
        new Range(' '),
        new Range('\t', '\r')
      };
      _nonWhitespaces = InvertFullRange(_whitespaces);

      _numbers = new[] {
        new Range('0', '9')
      };
      _nonNumbers = InvertFullRange(_numbers);

      _wordCharacters = new[] {
        new Range('a', 'z'),
        new Range('A', 'Z'),
        new Range('0', '9'),
        new Range('_')
      };
      _nonWordCharacters = InvertFullRange(_wordCharacters);
      _allExceptNewline = InvertFullRange(new[] { new Range('\n') });
    }

    private static string _pattern = string.Empty;
    private static int _position;
    private static Anchor _anchors;

    public static Pattern Build(string pattern)
    {
      _pattern = pattern;

      Rule[] parts = GetAllParts();
      Pattern result = new Pattern(parts, _anchors);

      _position = 0;
      _pattern = string.Empty;
      _anchors = Anchor.None;
      return result;
    }

    private static Rule[] GetAllParts()
    {
      Stack<RuleInfo> parts = new Stack<RuleInfo>(PATTERN_CAPACITY);

      while (_position < _pattern.Length) {
        GetPart(parts);
        _position++;
      }

      List<RuleInfo> list = new List<RuleInfo>(parts);

      //abc
      //for (int i = 0; i < list.Count - 1; i++)
      //{
      //  RuleInfo first = list[i]; //c
      //  RuleInfo second = list[i + 1]; //b

      //  if (second.IsGreedy) {
      //    Count count = (Count)second.Rule;
      //    if(count.GetClass() == null)
      //      continue;
      //    if(first.Rule.GetClass() == null)
      //      continue;
      //    first.Rule.GetClass()!.Exclude(count.GetClass()!.Ranges);
      //    if (first.Rule.GetClass()!.Ranges.Length == 0)
      //      list.RemoveAt(i--);
      //    //count.Exclude(first.Rule.GetClass()!.Ranges);
      //  }
      //}

      return parts.Reverse().Select(r => r.Rule).ToArray();
      //list.Reverse();
      //return list.Select(r => r.Rule).ToArray();
    }

    private static RuleInfo PopQuantifiableOrThrowException(Stack<RuleInfo> parts)
    {
      RuleInfo rule = PopOrThrowException(parts);
      if (!rule.Quantifiable) {
        throw new Exception("TODO");
      }

      return rule;
    }

    private static RuleInfo PopOrThrowException(Stack<RuleInfo> parts)
    {
      if (!parts.TryPop(out RuleInfo part)) {
        throw new ArgumentException($"parsing \"{_pattern}\" - Quantifier {{x,y}} following nothing.");
      }

      return part;
    }

    private static void GetPart(Stack<RuleInfo> parts)
    {
      Token token = GetToken();
      char currentChar = token.Value;
      switch (currentChar) {
        case '^' when IsStartOfLine(_position) && IsMetaCharacter(token):
        {
          _anchors |= Anchor.StartOfLine;
          break;
        }
        case '$' when IsEndOfLine(_position, _pattern.Length) && IsMetaCharacter(token):
        {
          _anchors |= Anchor.EndOfLine;
          break;
        }
        case '|' when IsInsideTheText(_position, _pattern.Length) && IsMetaCharacter(token):
        {
          RuleInfo left = parts.Pop();
          _position++;
          GetPart(parts);
          RuleInfo right = parts.Pop();
          Or or = new Or(left.Rule, right.Rule);
          parts.Push(new RuleInfo(or, true, false));
          break;
        }
        case 's' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_whitespaces);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case 'S' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_nonWhitespaces);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case 'd' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_numbers);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case 'D' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_nonNumbers);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case 'w' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_wordCharacters);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case 'W' when token.Type == TokenType.MetaEscape:
        {
          Class cl = new Class(_nonWordCharacters);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case '.' when token.Type == TokenType.MetaCharacter:
        {
          Class cl = new Class(_allExceptNewline);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
        case '*' when token.Type == TokenType.MetaCharacter:
        {
          RuleInfo rule = PopQuantifiableOrThrowException(parts);
          Count count = new Count(rule.Rule, 0, int.MaxValue);
          parts.Push(new RuleInfo(count, false, true));
          break;
        }
        case '+' when token.Type == TokenType.MetaCharacter:
        {
          RuleInfo rule = PopQuantifiableOrThrowException(parts);
          Count count = new Count(rule.Rule, 1, int.MaxValue);
          parts.Push(new RuleInfo(count, false, true));
          break;
        }
        case '?' when token.Type == TokenType.MetaCharacter:
        {
          RuleInfo rule = PopQuantifiableOrThrowException(parts);
          Count count = new Count(rule.Rule, 0, 1);
          parts.Push(new RuleInfo(count, false, false));
          break;
        }
        case '[' when token.Type == TokenType.MetaCharacter:
        {
          _position++;
          parts.Push(ParseClass());
          break;
        }
        case '(' when token.Type == TokenType.MetaCharacter:
        {
          _position++;
          parts.Push(ParseGroup());
          break;
        }
        case '{' when token.Type == TokenType.MetaCharacter:
        {
          RuleInfo rule = PopQuantifiableOrThrowException(parts);
          _position++;
          Count? count = ParseCount(rule.Rule);
          if (count == null) {
            parts.Push(rule);
            _position--;
            goto default;
          }

          parts.Push(new RuleInfo(count, false, count.IsGreedy));
          break;
        }
        default:
        {
          Range[] ranges = { new Range(currentChar) };
          Class cl = new Class(ranges);
          parts.Push(new RuleInfo(cl, true, false));
          break;
        }
      }
    }

    private static Count? ParseCount(Rule target)
    {
      int close = _pattern.IndexOf('}', _position);
      ReadOnlySpan<char> count = _pattern.AsSpan(_position, close - _position);

      if (count.Length == 0)
        return null;

      Span<char> buffer = stackalloc char[count.Length];
      ValueStringBuilder sb = new ValueStringBuilder(buffer);

      int first = -1;
      int second = -1;

      for (int i = 0; i < count.Length; i++) {
        char ch = count[i];
        if (char.IsNumber(ch)) {
          sb.Append(ch);
        }
        else if (ch == ',') {
          if (first == -1) {
            first = int.Parse(sb.GetSlice());
            second = int.MaxValue;
          }
          else {
            second = int.Parse(sb.GetSlice());
          }

          sb.Clear();
        }
        else {
          return null;
        }
      }

      if (first == -1) {
        first = int.Parse(sb.GetSlice());
        second = first;
      }

      if (sb.Length > 0) {
        second = int.Parse(sb.GetSlice());
      }

      _position += count.Length;
      return new Count(target, first, second);
    }

    private static RuleInfo ParseGroup()
    {
      int previousPosition = _position;
      string mainPattern = _pattern;

      int close = _pattern.IndexOf(')', _position);
      string groupSlice = _pattern.AsSpan(_position, close - _position).ToString();

      _pattern = groupSlice;
      _position = 0;

      Rule[] parts = GetAllParts();

      _position = groupSlice.Length + previousPosition;
      _pattern = mainPattern;
      Group group = new Group(parts);
      return new RuleInfo(group, true, false);
    }

    private static RuleInfo ParseClass()
    {
      int close = _pattern.IndexOf(']', _position);
      ReadOnlySpan<char> classSlice = _pattern.AsSpan(_position, close - _position);
      List<Range> ranges = new List<Range>(CLASS_CAPACITY);
      ParseClass(ranges, classSlice);
      Class cl = new Class(MergeRanges(ranges));
      _position += classSlice.Length;
      return new RuleInfo(cl, true, false);
    }

    private static void ParseClass(List<Range> rangeArray, ReadOnlySpan<char> cl)
    {
      Token previous = default;
      bool negativeClass = false;
      for (int i = 0; i < cl.Length; i++) {
        Token token = GetToken(cl, ref i);
        char currentChar = token.Value;
        switch (currentChar) {
          case '^' when IsStartOfLine(i):
            negativeClass = true;
            break;
          case '-' when IsMetaCharacter(token) && IsInsideTheText(i, cl.Length):
            int currentPosition = i++;
            Token next = GetToken(cl, ref i);
            if (previous.Type == TokenType.Symbol && next.Type == TokenType.Symbol) {
              rangeArray.Add(new Range(previous.Value, next.Value));
            }
            else {
              rangeArray.Add(new Range('-'));
              i = currentPosition;
            }

            break;
          default:
            AddRanges(rangeArray, token);
            break;
        }

        previous = token;
      }

      if (negativeClass) {
        Range[] result = ExcludeRanges(Range.Full, rangeArray.ToArray());
        rangeArray.Clear();
        rangeArray.AddRange(result);
      }
    }

    private static void AddRanges(List<Range> rangeArray, Token token)
    {
      char currentChar = token.Value;
      if (IsMetaEscape(token)) {
        switch (currentChar) {
          case 'w': rangeArray.AddRange(_wordCharacters); return;
          case 's': rangeArray.AddRange(_whitespaces); return;
          case 'd': rangeArray.AddRange(_numbers); return;
          case 'W': rangeArray.AddRange(InvertFullRange(_wordCharacters)); return;
          case 'S': rangeArray.AddRange(InvertFullRange(_whitespaces)); return;
          case 'D': rangeArray.AddRange(InvertFullRange(_numbers)); return;
          case 't': AddRangeToList(rangeArray, '\t'); return;
          case 'n': AddRangeToList(rangeArray, '\n'); return;
          case 'r': AddRangeToList(rangeArray, '\r'); return;
          case 'f': AddRangeToList(rangeArray, '\f'); return;
          case 'v': AddRangeToList(rangeArray, '\v'); return;
        }
      }
      else {
        AddRangeToList(rangeArray, currentChar);
      }

      //switch (currentChar) {
      //  case ' ' when token.Type == TokenType.Symbol:
      //  {
      //    currentChar = ' ';
      //  }
      //}
    }

    private static void AddRangeToList(List<Range> rangeArray, char currentChar)
    {
      if (rangeArray.Count == 0) {
        rangeArray.Add(new Range(currentChar));
      }
      else if (Range.TryInclude(rangeArray[^1], currentChar, out Range? newRange)) {
        rangeArray.RemoveAt(rangeArray.Count - 1);
        rangeArray.Add(newRange.Value);
      }
      else {
        rangeArray.Add(new Range(currentChar));
      }
    }

    private static Range[] InvertFullRange(Range[] ranges)
    {
      return ExcludeRanges(Range.Full, ranges);
    }

    private static Token GetToken()
    {
      return GetToken(_pattern, ref _position);
    }

    private static Token GetToken(ReadOnlySpan<char> chars, ref int position)
    {
      char currentChar = chars[position];
      return currentChar switch
      {
        '\\' => GetMetaEscapeOrSymbol(chars, ref position),
        '^' => new Token('^', TokenType.MetaCharacter),
        '$' => new Token('$', TokenType.MetaCharacter),
        '*' => new Token('*', TokenType.MetaCharacter),
        '+' => new Token('+', TokenType.MetaCharacter),
        '?' => new Token('?', TokenType.MetaCharacter),
        '[' => new Token('[', TokenType.MetaCharacter),
        '{' => new Token('{', TokenType.MetaCharacter),
        '(' => new Token('(', TokenType.MetaCharacter),
        '.' => new Token('.', TokenType.MetaCharacter),
        '-' => new Token('-', TokenType.MetaCharacter),
        '|' => new Token('|', TokenType.MetaCharacter),
        _ => new Token(currentChar)
      };
    }

    private static Token GetMetaEscapeOrSymbol(ReadOnlySpan<char> chars, ref int position)
    {
      char currentChar = chars[++position];
      return currentChar switch
      {
        'd' => new Token('d', TokenType.MetaEscape),
        'D' => new Token('D', TokenType.MetaEscape),
        'w' => new Token('w', TokenType.MetaEscape),
        'W' => new Token('W', TokenType.MetaEscape),
        's' => new Token('s', TokenType.MetaEscape),
        'S' => new Token('S', TokenType.MetaEscape),
        'n' => new Token('\n', TokenType.MetaEscape),
        'r' => new Token('\r', TokenType.MetaEscape),
        't' => new Token('\t', TokenType.MetaEscape),
        'f' => new Token('\f', TokenType.MetaEscape),
        'v' => new Token('\v', TokenType.MetaEscape),
        '0' => new Token('\0', TokenType.MetaEscape),
        'b' => throw new NotImplementedException(),
        'B' => throw new NotImplementedException(),
        'A' => throw new NotImplementedException(),
        'Z' => throw new NotImplementedException(),
        'z' => throw new NotImplementedException(),
        'G' => throw new NotImplementedException(),
        _ => new Token(currentChar)
      };

      //case 'b': return new Token('b', TokenType.MetaEscape);
      //case 'B': return new Token('B', TokenType.MetaEscape);
      //case 'A': return new Token('A', TokenType.MetaEscape);
      //case 'Z': return new Token('Z', TokenType.MetaEscape);
      //case 'z': return new Token('z', TokenType.MetaEscape);

      //case 'G': return new Token('G', TokenType.MetaEscape);
      // \cX
      // \xFF
      // \uFFFF
    }

    internal static Range[] MergeRanges(List<Range> ranges)
    {
      if (ranges.Count < 2)
        return ranges.ToArray();

      ranges.Sort();

      int skip = 0;
      while (true) {
        Range first = default;
        Range second = default;

        int count = 0;
        int skipped = 0;
        foreach (Range range in ranges) {
          if (skipped < skip) {
            skipped++;
            continue;
          }

          if (count == 0) {
            first = range;
          }
          else if (count == 1) {
            second = range;
          }
          else {
            break;
          }

          count++;
        }

        if (count <= 1)
          break;

        if (Range.TryMerge(first, second, out Range? result)) {
          ranges.Remove(first);
          ranges.Remove(second);
          ranges.Add(result.Value);
        }
        else {
          skip++;
        }
      }

      return ranges.ToArray();
    }

    internal static Range[] ExcludeRanges(Range target, Range[] ranges)
    {
      List<Range> result = new List<Range> { target };

      foreach (Range range in ranges) {
        List<Range> newResult = new List<Range>();

        foreach (Range current in result) {
          if (current.End < range.Start || current.Start > range.End) {
            newResult.Add(current);
          }
          else {
            if (current.Start < range.Start) {
              newResult.Add(new Range(current.Start, (char)(range.Start - 1)));
            }

            if (current.End > range.End) {
              newResult.Add(new Range((char)(range.End + 1), current.End));
            }
          }
        }

        result = newResult;
      }

      return MergeRanges(result);
    }

    private static bool IsStartOfLine(int position)
    {
      return position == 0;
    }

    private static bool IsEndOfLine(int position, int length)
    {
      return position + 1 == length;
    }

    private static bool IsMetaCharacter(Token token)
    {
      return token.Type == TokenType.MetaCharacter;
    }

    private static bool IsMetaEscape(Token token)
    {
      return token.Type == TokenType.MetaEscape;
    }

    private static bool IsInsideTheText(int position, int length)
    {
      return position != 0 && position != length - 1;
    }
  }
}