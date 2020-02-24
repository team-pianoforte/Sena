using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pianoforte.Solfege.Lang.Runtime
{
  public static class Operations
  {
    private static bool ValuesTypeIs(ValueType type, params Value[] values)
      => values.All((v) => v.Type == type);
    private static bool ValuesTypeIs(params (ValueType type, Value v)[] defs)
      => defs.All((def) => def.v.Type == def.type);
    private static bool ValueHasOneOfTypes(Value v, params ValueType[] types)
      => types.Any((t) => v.Type == t);
    private static bool OneOfValuesTypeIs(ValueType type, params Value[] values)
      => values.Any((v) => v.Type == type);
    private static bool OneOfValuesTypeIs(params (ValueType type, Value v)[] defs)
      => defs.Any((def) => def.v.Type == def.type);

    public static Value Add(Value lhs, Value rhs)
    {
      if (OneOfValuesTypeIs(ValueType.String, lhs, rhs))
      {
        return Value.MakeString(lhs.ToString() + rhs.ToString());
      }
      if (ValuesTypeIs(ValueType.Number, lhs, rhs))
      {
        return Value.MakeNumber(lhs.Number + rhs.Number);
      }
      if (ValuesTypeIs(ValueType.Array, lhs, rhs))
      {
        return Value.MakeArray(lhs.Array.Concat(rhs.Array));
      }
      throw new RuntimeException(string.Format(Properties.Resources.InvalidAddition, lhs, rhs));
    }

    public static Value Subtract(Value lhs, Value rhs)
    {
      if (ValuesTypeIs(ValueType.Number, lhs, rhs))
      {
        return Value.MakeNumber(lhs.Number - rhs.Number);
      }
      throw new RuntimeException(string.Format(Properties.Resources.InvalidSubtraction, lhs, rhs));
    }

    public static Value Multiple(Value lhs, Value rhs)
    {
      if (ValuesTypeIs(ValueType.Number, lhs, rhs))
      {
        return Value.MakeNumber(lhs.Number * rhs.Number);
      }

      var repeatable = ValueHasOneOfTypes(lhs, ValueType.String, ValueType.Array);
      if (repeatable && rhs.Type == ValueType.Number)
      {
        return Repeat(lhs, rhs);
      }

      throw new RuntimeException(string.Format(Properties.Resources.InvalidMultiplication, lhs, rhs));
    }
    public static Value Devide(Value lhs, Value rhs)
    {
      if (rhs.Type != ValueType.Number)
      {
        throw new RuntimeException(Properties.Resources.NonNumberDivision);
      }
      if (rhs.Number == 0)
      {
        throw new RuntimeException(Properties.Resources.DevideByZero);
      }

      switch (lhs.Type)
      {
        case ValueType.Number:
          return Value.MakeNumber(lhs.Number / rhs.Number);
      }
      throw new RuntimeException(string.Format(Properties.Resources.InvalidDivision, lhs, rhs));
    }

    public static Value Not(Value v)
    {
      if (v.Type != ValueType.Bool)
      {
        throw new RuntimeException(Properties.Resources.NonBoolNot);
      }
      return Value.MakeBool(!v.Bool);
    }

    public static Value Eq(Value lhs, Value rhs)
      => Value.MakeBool(lhs == rhs);

    public static Value NotEq(Value lhs, Value rhs)
      => Not(Eq(lhs, rhs));

    public static Value LessThan(Value lhs, Value rhs)
    {
      if (ValuesTypeIs(ValueType.Number, lhs, rhs))
      {
        return Value.MakeBool(lhs.Number < rhs.Number);
      }
      if (ValuesTypeIs(ValueType.String, lhs, rhs))
      {
        return Value.MakeBool(lhs.String.CompareTo(rhs.String) < 0);
      }
      return Value.MakeBool(false);
    }

    public static Value LessThanOrEquals(Value lhs, Value rhs)
      => Value.MakeBool(LessThan(lhs, rhs).Bool || Eq(lhs, rhs).Bool);

    public static Value GreaterThan(Value lhs, Value rhs)
    {
      if (ValuesTypeIs(ValueType.Number, lhs, rhs))
      {
        return Value.MakeBool(lhs.Number > rhs.Number);
      }
      if (ValuesTypeIs(ValueType.String, lhs, rhs))
      {
        return Value.MakeBool(lhs.String.CompareTo(rhs.String) > 0);
      }
      return Value.MakeBool(false);
    }

    public static Value GreaterThanOrEquals(Value lhs, Value rhs)
      => Value.MakeBool(GreaterThan(lhs, rhs).Bool || Eq(lhs, rhs).Bool);

    public static Value Length(Value v)
      => Value.MakeNumber(
        v.Type switch
        {
          ValueType.String => v.String.Length,
          ValueType.Array => v.Array.Length,
          _ => throw new RuntimeException(string.Format(Properties.Resources.InvalidLength, v)),
        }
      );

    public static Value Slice(Value v, Value start, Value end)
    {
      if (!(ValuesTypeIs(ValueType.Number, start, end)))
      {
        throw new RuntimeException(Properties.Resources.SliceByNonNumber);
      }

      var i = Math.Clamp(
        start.Integer < 0
          ? Length(v).Integer + start.Integer
          : start.Integer,
        0, Length(v).Integer - 1);
      var j = Math.Clamp(
        end.Integer < 0
          ? Length(v).Integer + end.Integer + 1  // End はそれ自体を含まないので、-1 のときのインデックスは Length - 1 ではなく Length になる
          : end.Integer,
        0, Length(v).Integer);
      return v.Type switch
      {
        ValueType.String => Value.MakeString(v.String.Substring(i, Math.Max(0, j - i))),
        ValueType.Array => Value.MakeArray(v.Array.Span(i, j)),
        _ => throw new RuntimeException(string.Format(Properties.Resources.InvalidSlice, v)),
      };
    }

    public static Value Repeat(Value v, Value n)
    {
      if (!(OneOfValuesTypeIs((ValueType.String, v), (ValueType.Array, v))))
      {
        throw new RuntimeException(string.Format(Properties.Resources.InvalidRepeat, v));
      }
      if (n.Type != ValueType.Number)
      {
        throw new RuntimeException(Properties.Resources.RepeatByNonNunmber);
      }
      if (n.Number == 0)
      {
        return v.Type == ValueType.String ? Value.MakeString("") : Value.MakeArray(new Array());
      }
      if (n.Number < 0)
      {
        return Repeat(Reverse(v), Value.MakeNumber(-n.Number));
      }

      var res = Enumerable.Range(0, (n.Integer) - 1).Aggregate(v, (sum, x) => Add(sum, v));
      if (!n.IsInteger)
      {
        res = Add(
          res,
          Slice(
            v,
            Value.MakeNumber(0),
            Multiple(Length(v), Subtract(n, Value.MakeNumber(n.Integer))))
          );
      }
      return res;
    }

    public static Value Reverse(Value v)
    {
      return v.Type switch
      {
        ValueType.String
          => Value.MakeString(string.Join("", v.String.ToCharArray().Reverse())),
        ValueType.Array
          => Value.MakeArray(new Array(v.Array.Reverse())),
        _ => throw new RuntimeException(string.Format(Properties.Resources.InvalidReverse, v)),
      };
    }

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

    public static Value InitArrayByTo(Value from, Value to, Value step)
    {
      if (!ValuesTypeIs(ValueType.Number, from, to, step))
      {
        throw new RuntimeException(Properties.Resources.NonNumberRange);
      }

      if (to.Number == from.Number)
      {
        // 1 to 1 step 0 => [1]
        return Value.MakeArray(new Array(new[] { from }));
      }

      if (step.Number == 0)
      {
        // 1 to 2 step 0 => Invalid
        throw new RuntimeException(Properties.Resources.StepIsZero);
      }

   
      if ((to.Number - from.Number) > 0 != step.Number > 0)
      {
        // 0 to 1 step 2 => [0]
        // 0 to 1 step -2  => []
        // 0 to 1 step 0.00001  => [0]
        // 0 to 1 step -0.00001  => []
        return Value.MakeArray(new Array());
      }

      var n = (int)((to.Number - from.Number) / step.Number) + 1;
      return Value.MakeArray(new Array(
        Enumerable.Range(0, n).Select((i) => Value.MakeNumber(from.Number + (i * step.Number))
      )));
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
