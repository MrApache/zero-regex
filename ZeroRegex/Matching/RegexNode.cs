namespace ZeroRegex
{
  internal abstract class RegexNode
  {
    private static ulong _id;

    protected readonly RegexNode? Node;
    public readonly bool Quantifiable;

    protected RegexNode(RegexNode? node, bool quantifiable)
    {
      Node = node;
      Quantifiable = quantifiable;
    }

    public abstract bool Evaluate(ref MatchContext context);
    public abstract string GenerateMethod(GeneratorContext context);
    public abstract RegexNode? Rebuild();

    /*
    public abstract bool CanMerge(RegexNode a);
    public abstract RegexNode Merge(RegexNode a);
    */

    protected static string CreateUniqueMethodName(string name)
    {
      return $"{name}_{_id++}";
    }
  }
}