namespace ZeroRegex
{
  internal sealed class Quantifier : RegexNode
  {
    private readonly int _min;
    private readonly int _max;

    public Quantifier(RegexNode regexNode, int x, int y) : base(regexNode, false)
    {
      _min = x;
      _max = y;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int count = 0;
      for (int j = 0; j < _max; j++) {
        if (Node!.Evaluate(ref context)) {
          count++;
        }
        else {
          break;
        }
      }

      return count >= _min && count <= _max;
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string method = Node!.GenerateMethod(context);
      context.InvokationList.Remove(method);

      string code =
        $@"
      int count = 0;
      for (int j = 0; j < {_max}; j++) {{
        if ({context.CreateMethodInvokation(method)}) {{
          count++;
        }}
        else {{
          break;
        }}
      }}
      return count >= {_min} && count <= {_max};";

      string name = CreateUniqueMethodName("FindMatchMultipleTimes");
      context.MethodDeclarations.Add(name, new Method(name, code));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode? Rebuild()
    {
      RegexNode? convertedNode = Node!.Rebuild();
      if (convertedNode == null)
        return null;
      if (convertedNode == Node)
        return this;

      return new Quantifier(convertedNode, _min, _max);
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return false;
    }
  */
  }
}