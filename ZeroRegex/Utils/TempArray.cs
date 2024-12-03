using System;

namespace ZeroRegex.Utils
{
  internal ref struct TempArray<T> where T : unmanaged, IEquatable<T>
  {
    private readonly Span<T> _buffer;
    private short _position;

    public readonly int Length => _position;
    public readonly int Size => _buffer.Length;

    public T this[int index]
    {
      get
      {
        if (index >= _position)
          throw new IndexOutOfRangeException();
        return _buffer[index];
      }
    }

    public TempArray(Span<T> buffer)
    {
      _buffer = buffer;
      _position = 0;
    }

    public bool Add(T item)
    {
      if (_position >= _buffer.Length)
        return false;

      _buffer[_position++] = item;
      return true;
    }

    public bool RemoveLast()
    {
      return RemoveLast(out T _);
    }

    public bool RemoveLast(out T value)
    {
      value = default;
      if (_position == 0)
        return false;
      value = _buffer[--_position];
      _buffer[_position] = default;
      return true;
    }

    public void RemoveAll(T value)
    {
      int start;
      while ((start = _buffer.IndexOf(value)) != -1) {
        for (int i = start; i < _position; i++) {
          if (i + 1 < _buffer.Length) {
            _buffer[i] = _buffer[i + 1];
          }
          else {
            _buffer[i] = default;
          }
        }

        _position--;
      }
    }

    public void Clear()
    {
      _position = 0;
    }

    public T GetLast()
    {
      if (_position <= 0)
        return default;

      return _buffer[_position - 1];
    }

    public Span<T> AsSpan(int start, int length)
    {
      return _buffer.Slice(start, length);
    }
  }
}