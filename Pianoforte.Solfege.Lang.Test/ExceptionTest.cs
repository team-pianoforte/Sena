using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Pianoforte.Solfege.Lang.Test
{
  public class ExceptionTest
  {

    [Fact]
    public void TestInternalAssertionException()
    {
      var ee = new Exception("err");
      var e = new InternalAssertionException("msg", ee);
      Assert.Equal("msg", e.Message);
      Assert.Equal(ee, e.InnerException);
    }

    [Fact]
    public void TestSyntaxErrorWithToken()
    {
      var pos = new TokenPosition("", 10, 10);
      var token = new Token(TokenKind.NoneLiteral, "none", pos);
      var e = new SyntaxException(token, "msg");
      Assert.Equal("msg", e.Message);
      Assert.Equal(token, e.Token);
      Assert.Equal(pos, e.Position);
    }

    [Fact]
    public void TestSyntaxErrorWithTokenPosition()
    {
      var pos = new TokenPosition("", 10, 10);
      var e = new SyntaxException(pos, "msg");
      Assert.Equal("msg", e.Message);
      Assert.Null(e.Token);
      Assert.Equal(pos, e.Position);
    }
  }
}
