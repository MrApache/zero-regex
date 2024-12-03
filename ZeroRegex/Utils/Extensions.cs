using System;
using System.Collections.Generic;

namespace ZeroRegex.Utils
{
  internal static class Extensions
  {
    public static T[] ToArray<T>(this ref TempArray<T> array) where T : unmanaged, IEquatable<T>
    {
      T[] tArray = new T[array.Length];
      for (int i = 0; i < array.Length; i++) {
        tArray[i] = array[i];
      }

      return tArray;
    }

    public static void AddRange<T>(this ICollection<T> source, TempArray<T> collection)
      where T : unmanaged, IEquatable<T>
    {
      for (int i = 0; i < collection.Length; i++) {
        source.Add(collection[i]);
      }
    }

    public static void AddRange<T>(this ref TempArray<T> array, IEnumerable<T> enumerable)
      where T : unmanaged, IEquatable<T>
    {
      foreach (T item in enumerable) {
        array.Add(item);
      }
    }
  }
}