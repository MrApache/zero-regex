namespace ZeroRegex
{
  internal sealed class StartOfLine : RegexNode
  {
    public StartOfLine() : base(null, false)
    {
    }

    public override bool Evaluate(ref MatchContext context)
    {
      return context.Start == 0 || context.Text[context.Start - 1] == '\n';
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string method = Node!.GenerateMethod(context);
      context.MethodDeclarations.Remove(method);
      string code =
//$@"if({context.CreateMethodInvokation(method)}) {{
        @$"return {context.StartIntVariable} == 0 || {context.TextVariable}[{context.StartIntVariable} - 1] == '\n';";
//}}
//return false;";
      string name = CreateUniqueMethodName("TryFindMatchAtStartOfLine");
      context.MethodDeclarations.Add(name, new Method(name, code));
      return name;
    }

    public override RegexNode? Rebuild()
    {
      return null;
      /*
      RegexNode? convertedNode = Node!.Convert();
      if (convertedNode == null)
        return null;
      if (convertedNode == Node)
        return this;

      return new Count(convertedNode, _min, _max);
    */
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return false;
    }
  */
  }
}