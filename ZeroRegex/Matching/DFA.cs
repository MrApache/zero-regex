namespace ZeroRegex
{
  internal sealed class DFA
  {
    private State _state;
    private bool _interrupt;

    public DFA(State initialState)
    {
      _state = initialState;
    }

    public void Evaluate(string input)
    {
      foreach (char ch in input) {
        if (_interrupt) {
          break;
        }
        _state.Evaluate(ch);
      }
    }

    public void Switch(State state)
    {
      _state = state;
    }

    public void Interrupt()
    {
      _interrupt = true;
    }
  }
}