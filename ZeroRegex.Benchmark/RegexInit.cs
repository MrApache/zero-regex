using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

// ReSharper disable once ObjectCreationAsStatement
namespace ZeroRegex.Benchmark
{
  [MemoryDiagnoser]
  public class RegexInit
  {
    private const string _pattern =
      @"((\w[^\W]+)[\.\-]?){1,}\@(([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

    [Benchmark]
    public void Default()
    {
      new Regex(_pattern);
    }

    [Benchmark]
    public void Custom()
    {
      PatternBuilder.Build(_pattern);
    }
  }
}