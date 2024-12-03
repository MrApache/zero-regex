using System;
using System.Runtime.CompilerServices;

namespace ZeroRegex.Utils
{
  internal ref struct ValueStringBuilder
  {
    public const byte IntMaxCharsCount = 11; // -2147483648;
    private const byte FirstNumberChar = 48; // '0'

    private Span<char> _buffer;
    private int _position;

    public int Length => _position;

    public ValueStringBuilder(Span<char> buffer)
    {
      _buffer = buffer;
      _position = 0;
    }

    public ValueStringBuilder(Span<char> buffer, int position)
    {
      if (position >= buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(position));

      _buffer = buffer;
      _position = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char value)
    {
      if (_position >= _buffer.Length) {
        Resize();
      }

      _buffer[_position++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(ReadOnlySpan<char> value)
    {
      if (value.Length + _position >= _buffer.Length) {
        Resize();
      }

      for (int i = 0; i < value.Length; i++) {
        _buffer[_position++] = value[i];
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(TempArray<char> array)
    {
      if (array.Length + _position >= _buffer.Length) {
        Resize();
      }

      for (int i = 0; i < array.Length; i++) {
        _buffer[_position++] = array[i];
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Resize()
    {
      char[] heapBuffer = new char[_buffer.Length * 2];
      for (int i = 0; i < _buffer.Length; i++) {
        heapBuffer[i] = _buffer[i];
      }

      _buffer = heapBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
      _position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
      return GetSlice().ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetSlice()
    {
      return _buffer[.._position];
    }
    
    public void Append(int value)
    {
      if (value == 0) {
        Append('0');
        return;
      }

      if (value < 0) {
        value *= -1;
        Append('-');
      }

      while (value > 0) {
        char ch = (char)(value % 10 + FirstNumberChar);
        Append(ch);
        value /= 10;
      }
    }    
  }
}