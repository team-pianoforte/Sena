using System;
using Xunit;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class ObjectTest
  {

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
