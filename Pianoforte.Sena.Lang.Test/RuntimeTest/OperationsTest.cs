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
      new object[] {
        Value.MakeArray(new Array(new[] { Value.MakeNumber(0), Value.MakeString("a"), Value.MakeNone() })),
        Value.MakeArray(new Array(new[] { Value.MakeNumber(0), Value.MakeString("a") })),
        Value.MakeArray(new Array(new[] { Value.MakeNone() } )),
      }
    };

    public static object[][] invalidAdditionData = {
      new object[] { Value.MakeNone(), Value.MakeNone() },
      new object[] { Value.MakeNumber(1), Value.MakeNone() },
    };

    [Theory]
    [MemberData(nameof(additionData))]

    public void TestAdd(Value expected, Value lhs, Value rhs)
    {
      Assert.Equal(expected, Operations.Add(lhs, rhs));
    }

    [Theory]
    [MemberData(nameof(invalidAdditionData))]
    public void TestInvalidAdd(Value lhs, Value rhs)
    {
      Assert.Throws<RuntimeException>(() => Operations.Add(lhs, rhs));
    }
  }
}
