namespace ZeroRegex
{
  internal sealed class Or : RegexNode
  {
    private readonly RegexNode _right;

    public Or(RegexNode left, RegexNode right) : base(left, true)
    {
      _right = right;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int start = context.Start;
      int length = context.Length;
      if (!Node!.Evaluate(ref context)) {
        context.Start = start;
        context.Length = length;
        return _right.Evaluate(ref context);
      }

      return true;
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string leftMethod = Node!.GenerateMethod(context);
      string rightMethod = _right.GenerateMethod(context);
      context.InvokationList.Remove(leftMethod);
      context.InvokationList.Remove(rightMethod);

      string code = $@"
      int tempStart = {context.StartIntVariable};
      int tempLength = {context.LengthIntVariable};
      if (!{context.CreateMethodInvokation(leftMethod)}) {{
        {context.StartIntVariable} = tempStart;
        {context.LengthIntVariable} = tempLength;
        return {context.CreateMethodInvokation(rightMethod)};
      }}
      return true;";
      string name = CreateUniqueMethodName("Or");
      context.MethodDeclarations.Add(name, new Method(name, code));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode? Rebuild()
    {
      RegexNode? left = Node!.Rebuild();
      RegexNode? right = _right.Rebuild();

      if (left == null && right == null)
        return null;

      if (left == null)
        return right;

      if (right == null)
        return left;

      return new Or(left, right);
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return false;
    }
  */
  }
}
