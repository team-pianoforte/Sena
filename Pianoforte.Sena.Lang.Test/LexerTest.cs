using System;
using Xunit;
using System.IO;
using System.Text;
using System.Linq;

namespace Pianoforte.Sena.Lang.Test
{
  public class LexerTest
  {
    // { input, kind, text, line, column }[]
    public static object[][] lexData = {
       new object[] { "", new[] { new Token(TokenKind.EndOfFile, "", new TokenPosition("", 1, 1))} },
    };

    [Theory]
    [MemberData(nameof(lexData))]
    public void TestLex(string input, Token[] expectedTokens)
    {
      var lexer = new Lexer("", new MemoryStream(Encoding.UTF8.GetBytes(input)));
      var tokenList = Enumerable.Zip(expectedTokens, lexer.Lex(), (a, b) => new { expected = a, actual = b });
      foreach(var t in tokenList)
      {
        Assert.Equal(t.expected, t.actual);
      }
    }
  }
}
