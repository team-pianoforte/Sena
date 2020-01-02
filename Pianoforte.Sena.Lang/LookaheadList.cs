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
    T[] buffer;
    int pos;
    readonly int capacity;

    public LookaheadList() { }
    public LookaheadList(int n)
    {
      capacity = n;
      Clear();
    }

    private int calcIndex(int i)
    {
      return (pos + i) % capacity;
    }

    public T this[int i]
    {
      get => buffer[calcIndex(i)];
      set => buffer[calcIndex(i)] = value;
    }

    public T Head { get => this[0]; }

    public void Clear()
    {
      buffer = new T[capacity];
    }

    public T Next()
    {
      var v = Head;
      pos = calcIndex(1);
      return v;
    }
  }
}
