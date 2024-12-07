using BenchmarkDotNet.Attributes;

namespace ZeroRegex.Benchmark
{
  [MemoryDiagnoser]
  public class RulePerformance
  {
    private const string _input = "\"Hello world\"";
    private readonly Char _char;
    private readonly Quantifier _quantifier;
    private readonly Class _class;

    public RulePerformance()
    {
      _char = new Char('"');
      Range[] ranges = Range.ExcludeRanges(Range.Full, new Range('"'));
      _class = new Class(ranges);
      _quantifier = new Quantifier(new Class(ranges), 0, int.MaxValue);
    }

    public static void Test()
    {
      RulePerformance performance = new RulePerformance();
      performance.CharByCharTest();
    }

    [Benchmark]
    public void Class()
    {
      MatchContext context = new MatchContext(_input);
      context.Start = 1;
      _class.Evaluate(ref context);
    }

    [Benchmark(Baseline = true)]
    public void CharByCharTest()
    {
      MatchContext context = new MatchContext(_input);
      _char.Evaluate(ref context);
    }

    [Benchmark]
    public void QuantifierTest()
    {
      MatchContext context = new MatchContext(_input);
      context.Start = 1;
      _quantifier.Evaluate(ref context);
    }
  }
}