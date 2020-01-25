using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class OperationsTest
  {
    public static object[][] additionData = {
      new object[] { Value.MakeNumber(2), Value.MakeNumber(1), Value.MakeNumber(1) },
      new object[] { Value.MakeString("ab"), Value.MakeString("a"), Value.MakeString("b") },
      new object[] { Value.MakeString("1b"), Value.MakeNumber(1), Value.MakeString("b") },
      new object[] { Value.MakeString("noneb"), Value.MakeNone(), Value.MakeString("b") },
      new object[] { Value.MakeString("trueb"), Value.MakeBool(true), Value.MakeString("b") },
      new object[] {
        Value.MakeArray(new Array(new[] { Value.MakeNumber(0), Value.MakeString("a"), Value.MakeNone() })),
        Value.MakeArray(new Array(new[] { Value.MakeNumber(0), Value.MakeString("a") })),
        Value.MakeArray(new Array(new[] { Value.MakeNone() } )),
      }
    };

    [Theory]
    [MemberData(nameof(additionData))]
    public void TestAdd(Value expected, Value lhs, Value rhs)
    {
      Assert.Equal(expected, Operations.Add(lhs, rhs));
    }

    [Fact]
    public void TestInvalidAdd()
    {
      Assert.Throws<RuntimeException>(() => Operations.Add(Value.MakeNone(), Value.MakeNumber(0)));
    }

    public static object[][] subtractionData = {
      new object[] { Value.MakeNumber(1), Value.MakeNumber(2), Value.MakeNumber(1) },
      new object[] { Value.MakeNumber(-1), Value.MakeNumber(1), Value.MakeNumber(2) },
    };

    [Theory]
    [MemberData(nameof(subtractionData))]
    public void TestSubtract(Value expected, Value lhs, Value rhs)
    {
      Assert.Equal(expected, Operations.Subtract(lhs, rhs));
    }

    [Fact]
    public void TestInvalidSubtract()
    {
      Assert.Throws<RuntimeException>(() =>
        Operations.Subtract(Value.MakeString(""), Value.MakeString("")));
    }

    [Fact]
    public void TestLength()
    {
      var (zero, two, none) = (Value.MakeNumber(0), Value.MakeNumber(2), Value.MakeNone());
      Assert.Equal(zero, Operations.Length(Value.MakeString("")));
      Assert.Equal(zero, Operations.Length(Value.MakeArray(new Array())));

      Assert.Equal(two, Operations.Length(Value.MakeString("aa")));
      Assert.Equal(two, Operations.Length(Value.MakeArray(new Array(new[] { none, none }))));
    }

    [Fact]
    public void TestSlice()
    {
      var str = Value.MakeString("abc");
      var arr = Value.MakeArray(new Array(new[] {
        Value.MakeString("a"),
        Value.MakeString("b"),
        Value.MakeString("c"),
      }));

      Assert.Equal(str, Operations.Slice(str, Value.MakeNumber(0), Value.MakeNumber(3)));
      Assert.Equal(str, Operations.Slice(str, Value.MakeNumber(0), Value.MakeNumber(-1)));
      Assert.Equal(str, Operations.Slice(str, Value.MakeNumber(-10), Value.MakeNumber(5)));
      Assert.Equal(arr, Operations.Slice(arr, Value.MakeNumber(0), Value.MakeNumber(3)));
      Assert.Equal(arr, Operations.Slice(arr, Value.MakeNumber(0), Value.MakeNumber(-1)));
      Assert.Equal(arr, Operations.Slice(arr, Value.MakeNumber(-10), Value.MakeNumber(5)));

      var ab = Value.MakeString("ab");
      var arr02 = Value.MakeArray(arr.Array.Span(0, 2));
      Assert.Equal(ab, Operations.Slice(str, Value.MakeNumber(0), Value.MakeNumber(2)));
      Assert.Equal(ab, Operations.Slice(str, Value.MakeNumber(-3), Value.MakeNumber(-2)));
      Assert.Equal(arr02, Operations.Slice(arr, Value.MakeNumber(0), Value.MakeNumber(2)));
      Assert.Equal(arr02, Operations.Slice(arr, Value.MakeNumber(-3), Value.MakeNumber(-2)));

      var bc = Value.MakeString("bc");
      var arr13 = Value.MakeArray(arr.Array.Span(1, 3));
      Assert.Equal(bc, Operations.Slice(str, Value.MakeNumber(1), Value.MakeNumber(3)));
      Assert.Equal(bc, Operations.Slice(str, Value.MakeNumber(-2), Value.MakeNumber(-1)));
      Assert.Equal(arr13, Operations.Slice(arr, Value.MakeNumber(1), Value.MakeNumber(3)));
      Assert.Equal(arr13, Operations.Slice(arr, Value.MakeNumber(-2), Value.MakeNumber(-1)));
    }

  }
}
