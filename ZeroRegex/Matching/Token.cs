using System;
using ZeroRegex.Utils;

namespace ZeroRegex
{
  internal readonly struct Token : IEquatable<Token>
  {
    public readonly char Value;
    public readonly TokenType Type;

    public Token(char token, TokenType type = TokenType.Symbol)
    {
      Value = token;
      Type = type;
    }

    public bool Equals(Token other)
    {
      return Value == other.Value
             && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
      return obj is Token token
             && Equals(token);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Value, Type);
    }

    public override string ToString()
    {
      string typeName = Type switch
      {
        TokenType.Symbol => nameof(TokenType.Symbol),
        TokenType.MetaCharacter => nameof(TokenType.MetaCharacter),
        _ => nameof(TokenType.MetaEscape)
      };

      int size = typeName.Length + 6;
      Span<char> temp = stackalloc char[size];
      ValueStringBuilder sb = new ValueStringBuilder(temp);
      sb.Append('[');
      sb.Append(typeName);
      sb.Append("] '");
      sb.Append(Value);
      sb.Append('\'');
      return sb.ToString();
    }
  }
}