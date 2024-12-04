namespace ZeroRegex
{
  internal interface IRuleBuilder
  {
    public bool Quantifiable { get; }
    public bool IsEmpty { get; }
    public Rule Build();
    public ClassBuilder? GetClassBuilder();
  }
}