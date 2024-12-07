using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace ZeroRegex.Utils
{
  internal readonly struct PoolBuffer<T> : IDisposable, ICollection<T>
  {
    private readonly T[] _array;
    private readonly bool _isPoolUsed;
    public int Count => _array.Length;
    public bool IsReadOnly => false;
    public bool RequireDispose => _isPoolUsed;

    public T this[int index]
    {
      get => _array[index];
      set => _array[index] = value;
    }

    public PoolBuffer(int size)
    {
      _array = ArrayPool<T>.Shared.Rent(size);
      _isPoolUsed = true;
    }

    public PoolBuffer(T[] array)
    {
      _array = array;
      _isPoolUsed = false;
    }

    public void Dispose()
    {
      if (_isPoolUsed) {
        ArrayPool<T>.Shared.Return(_array);
      }
    }

    public void Sort()
    {
      Array.Sort(_array);
    }

    public Enumerator GetEnumerator()
    {
      return new Enumerator(_array);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Add(T item)
    {
      throw new NotSupportedException();
    }

    public void Clear()
    {
      for (int i = 0; i < _array.Length; i++) {
        _array[i] = default!;
      }
    }

    public bool Contains(T item)
    {
      throw new NotSupportedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      throw new NotSupportedException();
    }

    public bool Remove(T item)
    {
      throw new NotSupportedException();
    }

    public static implicit operator PoolBuffer<T>(T[] array)
    {
      return new PoolBuffer<T>(array);
    }

    public struct Enumerator : IEnumerator<T>
    {
      private readonly T[] _array;
      private int _pointer;
      public T Current => _array[_pointer];
      object? IEnumerator.Current => Current;

      public Enumerator(T[] array)
      {
        _array = array;
        _pointer = 0;
      }

      public bool MoveNext()
      {
        if (_pointer + 1 >= _array.Length) {
          return false;
        }

        _pointer++;
        return true;
      }

      public void Reset()
      {
        _pointer = 0;
      }

      public void Dispose()
      {
      }
    }
  }
}