using System;

namespace ZeroRegex
{
  [Flags]
  internal enum Anchor : byte
  {
    None,
    StartOfLine,
    EndOfLine
  }
}