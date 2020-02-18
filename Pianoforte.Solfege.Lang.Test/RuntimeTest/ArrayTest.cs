using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Pianoforte.Solfege.Lang.Runtime.Test
{
  public class ArrayTest
  {
    [Fact]
    public void TestConstructorWithItems()
    {
      var v = Value.MakeNumber(42);
      var arr = new Array(new[] { v });
      Assert.Equal(new[] { v }, arr.ToArray());
    }

    [Fact]
    public void TestToString()
    {
      var arr = new Array(new[] { Value.MakeNumber(42), Value.MakeNone() });
      Assert.Equal("[42, none]", arr.ToString());
    }

    [Fact]
    public void TestSetItem()
    {
      var arr = new Array();
      arr.SetItem(2, Value.MakeNumber(42));
      Assert.Equal("[none, none, 42]", arr.ToString());
      arr.SetItem(0, Value.MakeBool(true));
      Assert.Equal("[true, none, 42]", arr.ToString());
    }

    [Fact]
    public void TestLength()
    {
      Assert.Equal(0, new Array().Length);
      Assert.Equal(10, new Array(10).Length);
      Assert.Equal(2, new Array(new[] { Value.MakeNone(), Value.MakeNone() }).Length);
    }
    [Fact]
    public void TestLengthGrowAndShrink()
    {
      var arr = new Array();
      Assert.Equal(0, arr.Length);
      arr.Length = 3;
      Assert.Equal(3, arr.Length);
      Assert.Equal("[none, none, none]", arr.ToString());

      arr.Length = 1;
      Assert.Equal(1, arr.Length);
      Assert.Equal("[none]", arr.ToString());
    }

    [Fact]
    public void TestNegativeLength()
    {
      var arr = new Array();
      Assert.Throws<RuntimeException>(() => arr.Length = -1);
    }

    [Fact]
    public void TestItemOutOfIndex()
    {
      var arr = new Array();
      Assert.Throws<RuntimeException>(() => arr.Item(1));
      Assert.Throws<RuntimeException>(() => arr.SetItem(-1, Value.MakeNone()));
      Assert.Throws<RuntimeException>(() => arr.Item(-1));
    }

    [Fact]
    public void TestCompare()
    {
      var (a, b, c) = (
        new Array(new[] { Value.MakeNone() }),
        new Array(new[] { Value.MakeNone() }),
        new Array(new Array(new[] { Value.MakeNumber(1), Value.MakeString("b") })));

      Assert.True(a == b);
      Assert.True(a.Equals(b));
      Assert.Equal(b, a);
      Assert.False(a != b);

      Assert.False(a == c);
      Assert.False(a.Equals(c));
      Assert.NotEqual(c, a);
      Assert.True(a != c);
    }


    public static object[][] spanData = new[]{
      new object[] { -1, 5, new[] { "a", "b", "c" } },
      new object[] { 0, 3, new[] { "a", "b", "c" } },
      new object[] { 0, 1, new[] { "a" } },
      new object[] { 1, 0, new string[] { } },
    };
    [Theory]
    [MemberData(nameof(spanData))]
    public void TestSpan(int i, int j, string[] ss)
    {
      var actual = new Array(new[] {
        Value.MakeString("a"),
        Value.MakeString("b"),
        Value.MakeString("c"),
      }).Span(i, j);
      var expected = new Array(ss.Select((s) => Value.MakeString(s)));
      Assert.Equal(expected.Length, actual.Length);
      Assert.Equal(expected, actual);
    }
  }
}
