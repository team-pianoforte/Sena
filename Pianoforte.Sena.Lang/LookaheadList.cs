using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  /// <summary>
  /// List for lookahead
  /// </summary>
  /// <typeparam name="T">Type of elements</typeparam>
  public class LookaheadList<T>
  {
    readonly T[] buffer;
    int head;
    readonly int capacity;
    int filledCount;

    public LookaheadList() { }
    public LookaheadList(int n)
    {
      buffer = new T[n];
      capacity = n;
    }

    public T this[int i]
    {
      get => Lookup(i);
    }

    private int calcBufferIndex(int i) => (head + i) % capacity;

    public T Lookup(int i)
    {
      if (filledCount < capacity && i >= filledCount)
      {
        throw new IndexOutOfRangeException();
      }
      return buffer[calcBufferIndex(i)];
    }

    public T Push(T v)
    {
      if (filledCount < capacity)
      {
        buffer[head] = v;
        filledCount += 1;
      }
      else
      {
        buffer[calcBufferIndex(-1)] = v; // Set to last
      }
      head = calcBufferIndex(1);
      return v;
    }

    public void Clear()
    {
      filledCount = 0;
    }
  }
}
