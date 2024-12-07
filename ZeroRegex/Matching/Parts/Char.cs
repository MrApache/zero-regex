using System;

namespace ZeroRegex
{
  internal sealed class Char : RegexNode
  {
    public readonly char Value;

    public Char(char ch) : base(null, true)
    {
      Value = ch;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      int pointer = context.Start + context.Length;
      if (pointer >= context.Text.Length)
        return false;
      return context.Text[context.Start + context.Length++] == Value;
    }

    public override string GenerateMethod(GeneratorContext context)
    {
      string code =
        $@"int pointer = {context.StartIntVariable} + {context.LengthIntVariable};
           if(pointer >= {context.TextVariable}.Length)
             return false;
           return {context.TextVariable}[{context.StartIntVariable} + {context.LengthIntVariable}++] == '{Value}';";

      string name = CreateUniqueMethodName("CharMatch");
      context.MethodDeclarations.Add(name, new Method(name, code));
      context.InvokationList.Add(name);
      return name;
    }

    public override RegexNode Rebuild()
    {
      return this;
    }

    /*
    public override bool CanMerge(RegexNode a)
    {
      return a is Char || a is CharSet;
    }

    public override RegexNode Merge(RegexNode a)
    {
      if (a is Char ch) {
        return new CharSet(Value, ch.Value);
      }

      if (a is CharSet set) {
        set.AppendToStart(Value);
        return a;
      }

      throw new Exception($"Unsupported type {a.GetType().Name}");
    }
  */
  }
}