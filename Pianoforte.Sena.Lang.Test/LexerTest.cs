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
      new object[] { @"""abc"" ""\\\""""" + "\n", new[] {
        new Token(TokenKind.StringLiteral, "abc", new TokenPosition("", 1, 1)),
        new Token(TokenKind.StringLiteral, @"\""", new TokenPosition("", 1, 7)),
        new Token(TokenKind.EndOfLine, "\n", new TokenPosition("", 1, 13)),
        new Token(TokenKind.EndOfFile, "", new TokenPosition("", 2, 1)),
      }},
      new object[] { "none true false value\n", new[] {
        new Token(Keywords.None.TokenKind, Keywords.None.Text, new TokenPosition("", 1, 1)),
        new Token(Keywords.True.TokenKind, Keywords.True.Text, new TokenPosition("", 1, 6)),
        new Token(Keywords.False.TokenKind, Keywords.False.Text, new TokenPosition("", 1, 11)),
        new Token(TokenKind.Identifier, "value", new TokenPosition("", 1, 17)),
        new Token(TokenKind.EndOfLine, "\n", new TokenPosition("", 1, 22)),
        new Token(TokenKind.EndOfFile, "", new TokenPosition("", 2, 1)),
      }},
    };

    [Theory]
    [MemberData(nameof(lexData))]
    public void TestLex(string input, Token[] expectedTokens)
    {
      var lexer = new Lexer("", new MemoryStream(Encoding.UTF8.GetBytes(input)));
      var tokenList = Enumerable.Zip(expectedTokens, lexer.Lex(), (a, b) => new { expected = a, actual = b });
      foreach (var t in tokenList)
      {
        Assert.Equal(t.expected, t.actual);
      }
    }
  }
}
