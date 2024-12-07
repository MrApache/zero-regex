using System;

namespace ZeroRegex
{
  #if NET5_0_OR_GREATER
  [AttributeUsage(AttributeTargets.Method
                  | AttributeTargets.Class
                  | AttributeTargets.Struct)]
  #else
  [AttributeUsage(AttributeTargets.Class
                  | AttributeTargets.Struct)]
#endif
  public sealed class CompileRegexAttribute : Attribute
  {
    public readonly string Pattern;
    public readonly bool Static;

    public CompileRegexAttribute(string pattern, bool useStatic = false)
    {
      Pattern = pattern;
      Static = useStatic;
    }
  }
}