using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace ZeroRegex.Benchmark;

[MemoryDiagnoser]
public class RegexPerformance
{
    private const string _pattern = "^\"[^\"]*\"";
    private readonly Regex _regex = new Regex(_pattern);
    //private readonly Matcher _matcher = new Matcher(_pattern);
    private readonly Pattern _pt = PatternBuilder.Build(_pattern);
    private readonly Regex _compiledRegex = new Regex(_pattern, System.Text.RegularExpressions.RegexOptions.Compiled);

    [Benchmark]
    public void SystemRegex()
    {
        _regex.IsMatch("Hello world");
    }

    [Benchmark]
    public void CompiledSystemRegex()
    {
        _compiledRegex.IsMatch("Hello world");
    }

    [Benchmark]
    public void CustomRegex()
    {
        _pt.Match("Hello world");
        //_matcher.IsMatch("Hello world");
    }

    [Benchmark]
    public void SystemRegexInit()
    {
        Regex regex = new Regex(_pattern);
    }

    [Benchmark]
    public void CompiledSystemRegexInit()
    {
        Regex compiledRegex = new Regex(_pattern, System.Text.RegularExpressions.RegexOptions.Compiled);
    }

    [Benchmark]
    public void CustomRegexInit()
    {
        Pattern pattern = PatternBuilder.Build(_pattern);
        //Matcher matcher = new Matcher(_pattern);
    }

    //| Method                  | Mean         | Error      | StdDev     | Gen0   | Gen1   | Allocated |
    //|------------------------ |-------------:|-----------:|-----------:|-------:|-------:|----------:|
    //| SystemRegex             |    45.022 ns |  0.2231 ns |  0.1978 ns |      - |      - |         - |
    //| CompiledSystemRegex     |    20.569 ns |  0.3844 ns |  0.3210 ns |      - |      - |         - |
    //| CustomRegex             |     8.273 ns |  0.0360 ns |  0.0336 ns |      - |      - |         - |
    //| SystemRegexInit         |   751.713 ns |  3.0914 ns |  2.7405 ns | 0.2289 |      - |    1440 B |
    //| CompiledSystemRegexInit | 5,179.242 ns | 97.5686 ns | 76.1752 ns | 1.1978 | 0.0229 |    7560 B |
    //| CustomRegexInit         |   527.952 ns |  2.8455 ns |  2.3761 ns | 0.2670 | 0.0010 |    1680 B |
}