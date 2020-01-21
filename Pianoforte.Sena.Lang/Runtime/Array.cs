using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Pianoforte.Sena.Lang.Runtime
{

  public class Array : IEnumerable<Value>
  {
    private readonly List<Value> values;
    public Array() : this(0) { }

    public Array(int n) : this(Enumerable.Repeat(Value.MakeNone(), n)) { }

    public Array(IEnumerable<Value> v)
    {
      values = new List<Value>(v);
    }

    public override string ToString()
      => "[" + string.Join(", ", values) + "]";


    private void ThrowIndexOutOfRange(int i)
    {
      throw new RuntimeException(string.Format(Properties.Resources.IndexOutOfRange, i));
    }

    private void AssertIndexIsPositive(int i)
    {
      if (i < 0)
      {
        ThrowIndexOutOfRange(i);
      }
    }

    private void AssertIndexInRange(int i)
    {
      if (!(0 <= i && i < Length))
      {
        ThrowIndexOutOfRange(i);
      }
    }

    public int Length
    {
      get => values.Count;
      set
      {
        AssertIndexIsPositive(value);
        if (value == Length)
        {
          return;
        }
        else if (value > Length)
        {
          values.AddRange(Enumerable.Repeat(Value.MakeNone(), value - Length));
        }
        else
        {
          values.RemoveRange(value, Length - value);
        }
      }
    }

    public void SetItem(int i, Value value)
    {
      AssertIndexIsPositive(i);
      if (i >= Length)
      {
        Length = i + 1;
      }
      values[i] = value;
    }

    public Value Item(int i)
    {
      AssertIndexInRange(i);
      return values[i];
    }

    public Array Concat(Array v)
      => new Array(Enumerable.Concat(this, v));
    

    public IEnumerator<Value> GetEnumerator()
    {
      return values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return values.GetEnumerator();
    }

    public override bool Equals(object obj)
    {
      return obj is Array array &&
             Length == array.Length &&
             Enumerable.All(Enumerable.Zip(values, array, (a, b) => (a, b)), (v) => v.a == v.b);
    }

    public static bool operator ==(Array lhs, Array rhs)
    {
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Array lhs, Array rhs)
    {
      return !(lhs == rhs);
    }

  }
}
