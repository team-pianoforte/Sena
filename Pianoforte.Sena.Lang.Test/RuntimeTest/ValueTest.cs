using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class ValueTest
  {

    [Fact]
    public void TestBool()
    {
      var v = Value.MakeBool(true);
      Assert.True(v.Bool);
      Assert.Equal(Value.TrueStr, v.ToString());
      Assert.Equal(v, v.ConvertType(ValueType.Bool));
      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Equal(Value.MakeString(Value.TrueStr).String, v.ConvertType(ValueType.String).String);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));

      Assert.True(Value.MakeString(Value.TrueStr).ConvertType(ValueType.Bool).Bool);

      v = Value.MakeBool(false);
      Assert.Equal(Value.FalseStr, v.ToString());

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Equal(v, v.ConvertType(ValueType.Bool));
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Number));
      Assert.Equal(Value.MakeString(Value.FalseStr).String, v.ConvertType(ValueType.String).String);

      Assert.False(Value.MakeString(Value.FalseStr).ConvertType(ValueType.Bool).Bool);

      Assert.Throws<InternalAssertionException>(() => v.Number);
      Assert.Throws<InternalAssertionException>(() => v.String);
    }

    [Fact]
    public void TestNumber()
    {
      var v = Value.MakeNumber(4.2m);
      Assert.Equal(4.2m, v.Number);
      Assert.Equal("4.2", v.ToString());

      Assert.Equal(ValueType.None, v.ConvertType(ValueType.None).Type);
      Assert.Throws<RuntimeException>(() => v.ConvertType(ValueType.Bool));
      Assert.Equal(v, v.ConvertType(ValueType.Number));
      Assert.Equal(Value.MakeString("4.2").String, v.ConvertType(ValueType.String).String);
      
      Assert.Equal(4.2m, Value.MakeString("4.2").ConvertType(ValueType.Number).Number);

      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.String);
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

      Assert.Throws<InternalAssertionException>(() => v.Bool);
      Assert.Throws<InternalAssertionException>(() => v.Number);
    }
  }
}
