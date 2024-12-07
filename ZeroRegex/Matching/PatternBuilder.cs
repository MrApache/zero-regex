namespace ZeroRegex
{
  public static class PatternBuilder
  {
    public static Pattern Build(string pattern)
    {
      RegexBuilder builder = new RegexBuilder(pattern);
      //Pattern pt = new Pattern(builder.ScanNodes(), Anchor.None);
      //GeneratorContext ct = new GeneratorContext("START", "LENGTH", "INPUT");
      //pt.GenerateCode(ct);
      return null!;
      //return pt;
    }
  }
}