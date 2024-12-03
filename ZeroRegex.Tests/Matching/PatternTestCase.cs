namespace RefinedShell.Tests.Matching;

internal readonly struct PatternTestCase
{
    public readonly string Pattern;
    public readonly string Example;
    public readonly string Result;

    public PatternTestCase(string pattern, string example, string result)
    {
        Pattern = pattern;
        Example = example;
        Result = result;
    }

    public override string ToString()
    {
        return $"'({Pattern})->{Example}'";
    }
}