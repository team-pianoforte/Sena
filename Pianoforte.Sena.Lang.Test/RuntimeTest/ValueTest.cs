using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class ValueTest
  {
    [Fact]
    public void TestEqual()
    {
      Assert.False(Value.MakeNone() != Value.MakeNone());
      Assert.True(Value.MakeNone() != Value.MakeNumber(0));

      var (obj, f) = (new Object("name"), new Function(null));

      Assert.True(Value.MakeNone() == Value.MakeNone());
      Assert.True(Value.MakeNumber(1) == Value.MakeNumber(1));
      Assert.True(Value.MakeString("a") == Value.MakeString("a"));
      Assert.True(
        Value.MakeArray(new Array(new[] { Value.MakeNone() })) ==
        Value.MakeArray(new Array(new[] { Value.MakeNone() }))
      );
      Assert.True(Value.MakeObject(obj) == Value.MakeObject(obj));
      Assert.True(Value.MakeFunction(f) == Value.MakeFunction(f));

    }


    [Fact]
    public void TestBool()
    {
      var v = Value.MakeBool(true);
      Assert.True(v.Bool);
      Assert.Equal(Keywords.True.Text, v.ToString());
      Assert.Equal(v, v.ConvertType(ValueType.Bool));
      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Equal(Value.MakeString(Keywords.True.Text).String, v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));

      Assert.True(Value.MakeString(Keywords.True.Text).ConvertType(ValueType.Bool).Bool);

      v = Value.MakeBool(false);
      Assert.Equal(Keywords.False.Text, v.ToString());

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Equal(v, v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(Value.MakeString(Keywords.False.Text).String, v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));

      Assert.False(Value.MakeString(Keywords.False.Text).ConvertType(ValueType.Bool).Bool);

      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.String);
      Assert.Throws<InternalAssertionException>(() => v.Array);
      Assert.Throws<InternalAssertionException>(() => v.Object);
      Assert.Throws<InternalAssertionException>(() => v.Function);
    }

    [Fact]
    public void TestNumber()
    {
      var v = Value.MakeNumber(4.2m);
      Assert.Equal(4.2m, v.Number);
      Assert.Equal(4, v.Integer);
      Assert.Equal("4.2", v.ToString());

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Equal(v, v.ConvertType(ValueType.Number));
      Assert.Equal(Value.MakeString("4.2").String, v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));

      Assert.Equal(4.2m, Value.MakeString("4.2").ConvertType(ValueType.Number).Number);

      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.String);
      Assert.Throws<InternalAssertionException>(() => v.Array);
      Assert.Throws<InternalAssertionException>(() => v.Object);
      Assert.Throws<InternalAssertionException>(() => v.Function);
    }
    [Fact]
    public void TestString()
    {
      var v = Value.MakeString("s");
      Assert.Equal("s", v.String);
      Assert.Equal("s", v.ToString());

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(v, v.ConvertType(ValueType.String));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));

      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.Array);
      Assert.Throws<InternalAssertionException>(() => v.Object);
      Assert.Throws<InternalAssertionException>(() => v.Function);
    }

    [Fact]
    public void TestObject()
    {
      var obj = new Object("v");
      var v = Value.MakeObject(obj);
      Assert.Equal("object v", v.ToString());


      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.String);
      Assert.Throws<InternalAssertionException>(() => v.Array);
      Assert.Equal(obj, v.Object);
      Assert.Throws<InternalAssertionException>(() => v.Function);

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(v.ToString(), v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Equal(v, v.ConvertType(ValueType.Object));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));
    }

    [Fact]
    public void TestArray()
    {
      var arr = new Array(2);
      var v = Value.MakeArray(arr);
      Assert.Equal("[none, none]", v.ToString());


      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.String);
      Assert.Equal(arr, v.Array);
      Assert.Throws<InternalAssertionException>(() => v.Object);
      Assert.Throws<InternalAssertionException>(() => v.Function);

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(v.ToString(), v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Equal(v, v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Function));
    }


    [Fact]
    public void TestFunction()
    {
      var expr = Expression.Lambda(Expression.Constant(null), "f", new ParameterExpression[] { });
      var f = new Function(expr);
      var v = Value.MakeFunction(f);
      Assert.Equal("func f()", v.ToString());


      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.String);
      Assert.Throws<InternalAssertionException>(() => v.Array);
      Assert.Throws<InternalAssertionException>(() => v.Object);
      Assert.Equal(f, v.Function);

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(v.ToString(), v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Array));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Object));
      Assert.Equal(v, v.ConvertType(ValueType.Function));
    }
  }
}
