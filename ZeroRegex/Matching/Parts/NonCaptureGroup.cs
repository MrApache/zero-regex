namespace ZeroRegex
{
  internal sealed class NonCaptureGroup : RegexNode
  {
    private readonly RegexNode[] _nodes;

    public NonCaptureGroup(RegexNode[] parts) : base(null, true)
    {
      _nodes = parts;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      foreach (RegexNode part in _nodes) {
        bool status = part.Evaluate(ref context);
        if (!status) return false;
      }

      return true;
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string invokationList = string.Empty;
      foreach (RegexNode rule in _nodes) {
        string method = rule.GenerateMethod(context);
        context.InvokationList.Remove(method);
        invokationList +=
          @$"if(!{context.CreateMethodInvokation(method)}) {{
            return false;
          }}";
        invokationList += '\n';
      }

      invokationList += "return true;";

      string name = CreateUniqueMethodName("EvaluateGroup");
      context.MethodDeclarations.Add(name, new Method(name, invokationList));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode Rebuild()
    {
      int freeIndex = -1;
      int removed = 0;
      for (int i = 0; i < _nodes.Length; i++) {
        RegexNode node = _nodes[i];
        RegexNode? convertedNode = node.Rebuild();
        if (convertedNode == null) {
          removed++;
          freeIndex = i;
        }
        else {
          if (freeIndex == -1) {
            _nodes[i] = node;
          }
          else {
            _nodes[freeIndex] = convertedNode;
            freeIndex = -1;
          }
        }
      }

      if (removed == 0) {
        return this;
      }

      int itemsLeft = _nodes.Length - removed;
      if (itemsLeft == 1) {
        return _nodes[0];
      }

      RegexNode[] nodes = new RegexNode[_nodes.Length - removed];
      for (int i = 0; i < nodes.Length; i++) {
        nodes[i] = _nodes[i];
      }
      return new NonCaptureGroup(nodes);
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return false;
    }
  */
  }
}
