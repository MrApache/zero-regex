using System;

namespace ZeroRegex
{
  internal sealed class CharSetOld : RegexNode
  {
    private string _chars;

    public CharSetOld(params char[] set) : base(null, false)
    {
      _chars = new string(set);
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int pointer = context.Start + context.Length;
      if (pointer >= context.Text.Length)
        return false;
      ReadOnlySpan<char> slice = context.Text.Slice(context.Start, _chars.Length);
      context.Length += _chars.Length;
      return slice.SequenceEqual(_chars);
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string code =
$@"int pointer = {context.StartIntVariable} + {context.LengthIntVariable};
if(pointer >= {context.TextVariable}.Length)
  return false;
const string set = {_chars};
ReadOnlySpan<char> slice = {context.TextVariable}.Slice({context.StartIntVariable}, {_chars.Length});
{context.LengthIntVariable} += {_chars.Length};
return slice.SequenceEqual(set);";

      string name = CreateUniqueMethodName("CompareCharSet");
      context.RegisterMethod(new Method(name, code));
      return name;
    }

    public override RegexNode Rebuild()
    {
      return this;
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return a is Char || a is CharSet;
    }

    public override RegexNode Merge(RegexNode a)
    {
      if (a is Char ch) {
        AppendToEnd(ch.Value);
        return this;
      }

      if (a is CharSet set) {
        _chars += set._chars;
        return this;
      }

      throw new Exception($"Unsupported type {a.GetType().Name}");
    }
    */

    public void AppendToStart(char ch)
    {
      Span<char> buffer = stackalloc char[_chars.Length + 1];
      for (int i = 1; i < _chars.Length; i++) {
        buffer[i] = _chars[i];
      }

      buffer[0] = ch;
      _chars = buffer.ToString();
    }

    private void AppendToEnd(char ch)
    {
      Span<char> buffer = stackalloc char[_chars.Length + 1];
      for (int i = 0; i < _chars.Length; i++) {
        buffer[i] = _chars[i];
      }

      buffer[_chars.Length] = ch;
      _chars = buffer.ToString();
    }
  }
}