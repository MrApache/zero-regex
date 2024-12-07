using BenchmarkDotNet.Running;

namespace ZeroRegex.Benchmark;

public static class Program
{
  public static void Main()
  {
    BenchmarkRunner.Run<RegexInit>();
  }
  
  //| Method  | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
  //|-------- |---------:|----------:|----------:|-------:|-------:|----------:|
  //| Default | 5.406 us | 0.0432 us | 0.0383 us | 1.1063 | 0.0153 |   6.82 KB |
  //| Custom  | 6.074 us | 0.0774 us | 0.0724 us | 1.3046 | 0.0153 |   8.02 KB |
}