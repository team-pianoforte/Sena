using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public static class Operations
  {
    public static Value Add(Value lhs, Value rhs)
    {
      if (lhs.Type == ValueType.String || rhs.Type == ValueType.String)
      {
        return Value.MakeString(lhs.ToString() + rhs.ToString());
      }
      if (lhs.Type == ValueType.Number && rhs.Type == ValueType.Number)
      {
        return Value.MakeNumber(lhs.Number + rhs.Number);
      }
      if (lhs.Type == ValueType.Array && rhs.Type == ValueType.Array)
      {
        return Value.MakeArray(lhs.Array.Concat(rhs.Array));
      }
      throw new RuntimeException(string.Format(Properties.Resources.InvalidAddition, lhs, rhs));
    }

    public static Value Length(Value v)
      => Value.MakeNumber(
        v.Type switch
        {
          ValueType.String => v.String.Length,
          ValueType.Array => v.Array.Length,
          _ => throw new RuntimeException(string.Format(Properties.Resources.InvalidLength, v)),
        }
      );

    public static Value MemberAccess(Value receiver, string name)
    {
      if (receiver.Type != ValueType.Object)
      {
        throw new RuntimeException(Properties.Resources.NonObjectMemberAccess);
      }
      return receiver.Object.Member(name);
    }

    public static Value FunctionCall(Value f, params Value[] args)
    {
      if (f.Type != ValueType.Function)
      {
        throw new RuntimeException(Properties.Resources.NonFunctionCalling);
      }
      return f.Function.Call(args);
    }

    public static Value InitArray(params Value[] items)
    {
      return Value.MakeArray(new Array(items));
    }

    private static void AssertTypeOfArrayAndIndex(Value v, Value index)
    {
      if (v.Type != ValueType.Array)
      {
        throw new RuntimeException(Properties.Resources.NonArrayIndexAccess);
      }
      if (!index.IsInteger)
      {
        throw new RuntimeException(Properties.Resources.NonIntArrayIndex);
      }
    }

    public static Value ArrayItem(Value v, Value index)
    {
      AssertTypeOfArrayAndIndex(v, index);
      return v.Array.Item((int)index.Number);
    }

    public static void SetArrayItem(Value v, Value index, Value e)
    {
      AssertTypeOfArrayAndIndex(v, index);
      v.Array.SetItem((int)index.Number, e);
    }
  }
}
