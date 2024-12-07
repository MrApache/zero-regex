using System;
using System.Collections.Generic;
using System.Linq;
using ZeroRegex.Utils;

namespace ZeroRegex
{
  internal ref struct RegexBuilder
  {
    private static readonly Range[] _whitespaces;
    private static readonly Range[] _nonWhitespaces;
    private static readonly Range[] _wordCharacters;
    private static readonly Range[] _nonWordCharacters;
    private static readonly Range[] _numbers;
    private static readonly Range[] _nonNumbers;
    private static readonly Range[] _allExceptNewline;

    static RegexBuilder()
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

    private readonly Stack<RegexNode> _nodes;
    private readonly ReadOnlySpan<char> _pattern;
    private int _position;
    private bool _startOfLineAnchor;
    private bool _endOfLineAnchor;

    public RegexBuilder(ReadOnlySpan<char> pattern)
    {
      _pattern = pattern;
      _nodes = new Stack<RegexNode>(16);
      _position = 0;
      _startOfLineAnchor = false;
      _endOfLineAnchor = false;
    }
    
    public RegexNode[] ScanNodes()
    {
      while (_position < _pattern.Length) {
        GetNode();
        _position++;
      }

      return Optimize();
    }

    private void InsertAnchorNodes(List<RegexNode> nodes)
    {
      if (_startOfLineAnchor) {
        nodes.Insert(1, new StartOfLine());
      }

      if (_endOfLineAnchor) {
        nodes.Insert(nodes.Count - 1, new EndOfLine());
      }
    }

    private RegexNode[] Optimize()
    {
      List<RegexNode> fixedNodes = FixGreedQuantifiers();
      for (int i = 0; i < fixedNodes.Count; i++) {
        fixedNodes[i] = fixedNodes[i].Rebuild()!;
      }
      fixedNodes.TrimExcess();
      fixedNodes.Reverse();
      InsertAnchorNodes(fixedNodes);
      return fixedNodes.ToArray();
    }

    private List<RegexNode> FixGreedQuantifiers()
    {
      List<RegexNode> list = new List<RegexNode>(_nodes);

      for (int i = 0; i < list.Count - 1; i++)
      {
        RegexNode first = list[i];
        RegexNode second = list[i + 1];

        if (!second.Quantifiable) {
          /*
          ClassBuilder? secondClass = second.GetClassBuilder();
          if(secondClass == null)
            continue;
          ClassBuilder? firstClass = first.GetClassBuilder();
          if(firstClass == null)
            continue;
          first.CanMerge(a)
          firstClass.Exclude(secondClass.Ranges.ToArray());
          */
        }
      }

      return list;
    }

    private void GetNode()
    {
      Token token = GetToken();
      char currentChar = token.Value;
      if (IsMetaEscape(token)) {
        switch (currentChar) {
          case 's': PushClass(_whitespaces); break;
          case 'S': PushClass(_nonWhitespaces); break;
          case 'd': PushClass(_numbers); break;
          case 'D': PushClass(_nonNumbers); break;
          case 'w': PushClass(_wordCharacters); break;
          case 'W': PushClass(_nonWordCharacters); break;
          default: _nodes.Push(new Char(currentChar)); break;
        }
      }
      else if (IsMetaCharacter(token)) {
        if (IsValidStartOfLineAnchor(token)) {
          _startOfLineAnchor = true;
          return;
        }
        if (IsValidEndOfLineAnchor(token)) {
          _endOfLineAnchor = true;
          return;
        }
        if (IsValidOrToken(token)) {
          PushOr();
          return;
        }

        switch (currentChar) {
          case '*': PushQuantifier(0, int.MaxValue); break;
          case '+': PushQuantifier(1, int.MaxValue); break;
          case '?': PushQuantifier(0, 1); break;
          case '.': _nodes.Push(new AnyChar()); break;
          case '[': PushClass(); break;
          case '(': PushGroup(); break;
          case '{': PushCustomQuantifierOrChar(currentChar); break;
          default: _nodes.Push(new Char(currentChar)); break;
        }
      }
      else {
        _nodes.Push(new Char(currentChar));
      }
    }


    private RegexNode PopQuantifiableOrThrowException()
    {
      RegexNode node = PopOrThrowException();
      if (!node.Quantifiable) {
        throw new Exception("TODO");
      }

      return node;
    }

    private RegexNode PopOrThrowException()
    {
      if (!_nodes.TryPop(out RegexNode node)) {
        throw new ArgumentException($"parsing \"{_pattern.ToString()}\" - Quantifier {{x,y}} following nothing.");
      }

      return node;
    }

    private void PushQuantifier(int min, int max)
    {
      RegexNode node = PopQuantifiableOrThrowException();
      Quantifier quantifier = new Quantifier(node, min, max);
      _nodes.Push(quantifier);
    }

    private void PushClass(Range[] ranges)
    {
      Class cl = new Class(ranges);
      _nodes.Push(cl);
    }

    private void PushCustomQuantifierOrChar(char currentChar)
    {
      RegexNode node = PopQuantifiableOrThrowException();
      _position++;
      Quantifier? quantifier = ParseCustomQuantifier(node);
      if (quantifier == null) {
        _position--;
        _nodes.Push(node);
        _nodes.Push(new Char(currentChar));
        return;
      }

      _nodes.Push(quantifier);
    }

    private void PushOr()
    {
      NonCaptureGroup left = new NonCaptureGroup(_nodes.Reverse().ToArray());
      _nodes.Clear();
      RegexNode right = ParseRightSide();
      _nodes.Push(new Or(left, right));
    }

    private void PushClass()
    {
      _position++;
      _nodes.Push(ParseClass());
    }

    private void PushGroup()
    {
      _position++;
      _nodes.Push(ParseGroup());
    }

    private Class ParseClass()
    {
      int close = _pattern.IndexOf(']', _position);
      ReadOnlySpan<char> classSlice = _pattern.Slice(_position, close);
      List<Range> ranges = new List<Range>();
      ParseClass(ranges, classSlice);
      Class cl = new Class(Range.MergeRanges(ranges));
      _position += classSlice.Length;
      return cl;
    }

    private void ParseClass(List<Range> rangeArray, ReadOnlySpan<char> cl)
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
        Range[] result = Range.ExcludeRanges(Range.Full, rangeArray.ToArray());
        rangeArray.Clear();
        rangeArray.AddRange(result);
      }
    }

    private RegexNode ParseRightSide()
    {
      return ParseGroup(++_position, _pattern.Length - _position);
    }

    private NonCaptureGroup ParseGroup()
    {
      return ParseGroup(_position, GetEndOfGroup() - _position);
    }

    private NonCaptureGroup ParseGroup(int start, int length)
    {
      ReadOnlySpan<char> slice = _pattern.Slice(start, length);
      RegexBuilder regexBuilder = new RegexBuilder(slice);
      RegexNode[] nodes = regexBuilder.ScanNodes();
      _position += slice.Length;
      return new NonCaptureGroup(nodes);
    }

    private Quantifier? ParseCustomQuantifier(RegexNode target)
    {
      int close = _pattern.IndexOf('}', _position);
      ReadOnlySpan<char> count = _pattern.Slice(_position, close);

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
      return new Quantifier(target, first, second);
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

    private bool IsValidStartOfLineAnchor(Token token)
    {
      return token.Value == '^'
             && IsStartOfLine(_position)
             && IsMetaCharacter(token);
    }

    private bool IsValidEndOfLineAnchor(Token token)
    {
      return token.Value == '$'
             && IsEndOfLine(_position, _pattern.Length)
             && IsMetaCharacter(token);
    }

    private bool IsValidOrToken(Token token)
    {
      return token.Value == '|'
             && IsInsideTheText(_position, _pattern.Length)
             && IsMetaCharacter(token);
    }

    private int GetEndOfGroup()
    {
      int count = 1;
      ReadOnlySpan<char> groupSlice = _pattern[_position..];

      for (int i = 0; i < groupSlice.Length; i++) {
        Token token = GetToken(groupSlice, ref i);
        char currentChar = token.Value;
        switch (currentChar) {
          case '(' when token.Type == TokenType.MetaCharacter:
            count++;
            break;
          case ')' when token.Type == TokenType.Symbol:
          {
            if (count == 1) {
              return i + _position;
            }
            count--;
            break;
          }
        }
      }

      throw new Exception($"Incomplete group structure at pos {_position - 1}");
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
      return Range.ExcludeRanges(Range.Full, ranges);
    }

    private Token GetToken()
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
  }
}