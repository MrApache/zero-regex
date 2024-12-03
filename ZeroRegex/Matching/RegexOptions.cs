using System;

namespace ZeroRegex
{
  [Flags]
  public enum RegexOptions
  {
    None = 0,
    IgnoreCase = 1,
    Multiline = 2,
    ExplicitCapture = 4,
    Singleline = 8,
    IgnorePatternWhitespace = 16,
    //RightToLeft = 32,
    //ECMAScript = 128,
    CultureInvariant = 256,
    //NonBacktracking = 512
  }
}