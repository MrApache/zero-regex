using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
namespace ZeroRegex.Benchmark
{
  [SimpleJob(RuntimeMoniker.Net80)]
  [MemoryDiagnoser]
  public partial class RegexPerformance
  {
    private const string _pattern =
      @"^[\w\.\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}$";
      //@"((\w[^\W]+)[\.\-]?){1,}\@(([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
      //"\"[^\"]*\"";

    private const string _input =
      "john.doe@my-company.uk";
      //"lama.loca.loca123@inca.com";
      //"\"Hello world\"";
    private readonly Regex _regex = new Regex(_pattern);
    private readonly Regex _compiledRegex = new Regex(_pattern, System.Text.RegularExpressions.RegexOptions.Compiled);
    private readonly Regex _generatedRegex = MyRegex();
    private readonly Pattern _pt = PatternBuilder.Build(_pattern);

    [Benchmark]
    public void SystemRegex()
    {
      _regex.Match(_input);
    }

    [Benchmark]
    public void SystemRegexValueMatch()
    {
      _regex.EnumerateMatches(_input).MoveNext();
    }

    [Benchmark]
    public void CompiledSystemRegex()
    {
      _compiledRegex.Match(_input);
    }

    [Benchmark]
    public void GeneratedSystemRegex()
    {
      _generatedRegex.Match(_input);
    }

    [Benchmark]
    public void CustomRegex()
    {
      _pt.Match(_input);
    }

    [Benchmark]
    public void GeneratedCustomStaticRegex()
    {
      StaticRegex.Match(_input);
    }

    [Benchmark]
    public void GeneratedCustomInstanceRegex()
    {
      new InstanceRegex().Match(_input);
    }

    [GeneratedRegex(_pattern)]
    private static partial Regex MyRegex();
    }

  [CompileRegex(@"^[\w\.\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}$", true)]
  public readonly ref partial struct StaticRegex;

  [CompileRegex(@"^[\w\.\-]+@[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}$")]
  public readonly ref partial struct InstanceRegex;
}
