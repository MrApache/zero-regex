using NUnit.Framework;

namespace ZeroRegex.Tests;

internal sealed class MatchingTest
{

    [Test]
    public void Test()
    {
        Pattern orr = PatternBuilder.Build("((a|b)+)(c|d)+");
        Match mtor = orr.Match("ababd");

        //Pattern pattern = PatternBuilder.Build(@"[\w]+[\d]+[!@#]?");
        //Match match = pattern.Match("abc123!");

        //Pattern pt = PatternBuilder.Build(@"[abc]+\d*[^a-z]");
        //Match mt = pt.Match("abc123@");


        //Pattern tt = PatternBuilder.Build(@"[a-z]");
        //Match ttm = tt.Match("ab");
    }
}
