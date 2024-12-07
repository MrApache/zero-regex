using System.Collections.Generic;

namespace ZeroRegex
{
  internal abstract class State
  {
    private readonly DFA _dfa;
    private readonly Dictionary<Range, State> _transitions;

    protected State(DFA dfa)
    {
      _dfa = dfa;
      _transitions = new Dictionary<Range, State>();
    }

    public void Evaluate(char c)
    {
      Range range = new Range(c);
      if (_transitions.TryGetValue(range, out State state)) {
        _dfa.Switch(state);
      }
      else {
        _dfa.Interrupt();
      }
    }

    public void AddTransition(char ch, State state)
    {
      _transitions.Add(new Range(ch), state);
    }

    public void AddTransition(State state, params Range[] ranges)
    {
      foreach (Range range in ranges) {
        _transitions.Add(range, state);
      }
    }
  }
}