using System;
using Xunit;
using System.Collections.Generic;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class ObjectTest
  {
    [Fact]
    public void TestConstructorWithMembers()
    {
      var v = Value.MakeNumber(42);
      var members = new Dictionary<string, Value>() { { "v", v} };
      var obj = new Object("name", members);
      Assert.Equal(v, obj.Member("v"));
    }

    [Fact]
    public void TestToString()
    {
      var obj = new Object("name");
      Assert.Equal("object name", obj.ToString());
    }

    [Fact]
    public void TestMemberSetThenGet()
    {
      var obj = new Object("name");
      var v = Value.MakeString("v");
      obj.SetMember("v", v);
      Assert.Equal(v, obj.Member("v"));
    }

    [Fact]
    public void TestMemberNotExists()
    {
      var obj = new Object("name");
      Assert.Throws<RuntimeException>(() => obj.Member("v"));
    }
  }
}
