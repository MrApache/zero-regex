using NUnit.Framework;

namespace ZeroRegex.Tests;

internal sealed class MatchingTest
{
  private const string _pattern =
    "^[-+]?[0-9]+([.][0-9]*)?";

  private const string _input = "-100.32";

  [Test]
  public void Test()
  {
    Pattern orr = PatternBuilder.Build(_pattern);
    Match mtor = orr.Match(_input);
  }
}