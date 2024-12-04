using System.Linq;

namespace ZeroRegex
{
  internal sealed class Group : Rule
  {
    private readonly Rule[] _rules;

    public Group(Rule[] parts)
    {
      _rules = parts;
    }

    public override bool Evaluate(ref MatchContext context)
    {
      foreach (Rule part in _rules) {
        bool status = part.Evaluate(ref context);
        if (!status) return false;
      }

      return true;
    }
  }

  internal sealed class GroupBuilder : IRuleBuilder
  {
    public bool Quantifiable => true;
    public bool IsEmpty => _builders.Length == 0;
    private readonly IRuleBuilder[] _builders;

    public GroupBuilder(params IRuleBuilder[] builders)
    {
      _builders = builders;
    }

    public Rule Build()
    {
      return new Group(_builders.Select(b => b.Build()).ToArray());
    }

    public ClassBuilder? GetClassBuilder()
    {
      for (int i = _builders.Length - 1; i != 0; i--) {
        IRuleBuilder builder = _builders[i];
        if (!builder.Quantifiable) {
          return builder.GetClassBuilder();
        }
      }
      return null;
    }
  }
}
