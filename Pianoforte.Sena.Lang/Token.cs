using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public enum TokenKind
  {
    Noop,
    NoneLiteral,
    TrueLiteral,
    FalseLiteral,
    StringLiteral,
    NumberLiteral,
    EndOfLine,
    EndOfFile,
    Identifier,

    OpAssignment,
    OpPlusAssignment,
    OpMinusAssignment,
    OpMultiplicationAssignment,
    OpDivisionAssignment,
    OpEqual,
    OpNotEqual,
    OpLessThan,
    OpLessThanOrEqual,
    OpGreaterThan,
    OpGreaterThanOrEqual,
    OpPlus,
    OpMinus,
    OpMultiplication,
    OpDivision,
    ParenLeft,
    ParenRight,
    SquareBracketLeft,
    SquareBracketRight,
  }

  public static class TokenGroups
  {
    public static bool IsAssignment(this Token v) => Assigments.Contains(v.Kind);
    public static readonly List<TokenKind> Assigments = new List<TokenKind>() { 
      TokenKind.OpAssignment,
      TokenKind.OpPlusAssignment,
      TokenKind.OpMinusAssignment,
      TokenKind.OpMultiplicationAssignment,
      TokenKind.OpDivisionAssignment,
    };

    public static bool IsTermOp(this Token v) => Assigments.Contains(v.Kind);
    public static readonly List<TokenKind> TermOps = new List<TokenKind>() {
      TokenKind.OpPlus,
      TokenKind.OpMinus,
    };

    public static bool IsFactorOp(this Token v) => Assigments.Contains(v.Kind);
    public static readonly List<TokenKind> FactorOps = new List<TokenKind>() {
      TokenKind.OpMultiplication,
      TokenKind.OpDivision,
    };

    public static bool IsComparesionOp(this Token v) => Assigments.Contains(v.Kind);
    public static readonly List<TokenKind> ComparesionOps = new List<TokenKind>() {
      TokenKind.OpEqual,
      TokenKind.OpNotEqual,
      TokenKind.OpLessThan,
      TokenKind.OpLessThanOrEqual,
      TokenKind.OpGreaterThan,
      TokenKind.OpGreaterThanOrEqual,
    };

    public static bool IsLiteral(this Token v) => Assigments.Contains(v.Kind);
    public static readonly List<TokenKind> Literals = new List<TokenKind>() {
      TokenKind.NoneLiteral,
      TokenKind.TrueLiteral,
      TokenKind.FalseLiteral,
      TokenKind.TrueLiteral,
    };
  }

  public readonly struct Keyword
  {
    public readonly string Text;
    public readonly TokenKind TokenKind;

    public Keyword(string text, TokenKind tokenKind)
    {
      Text = text;
      TokenKind = tokenKind;
    }
  }

  public static class Keywords
  {
    public static readonly Keyword None = new Keyword("none", TokenKind.NoneLiteral);
    public static readonly Keyword True = new Keyword("true", TokenKind.TrueLiteral);
    public static readonly Keyword False = new Keyword("false", TokenKind.TrueLiteral);

    public static readonly Dictionary<string, Keyword> Map = new Dictionary<string, Keyword> {
      { None.Text, None },
      { True.Text, True },
      { False.Text, False },
    };
  }

  public struct TokenPosition
  {
    public string InputName;
    public int Line, Column;

    public TokenPosition(string inputName, int line, int column)
    {
      InputName = inputName;
      Line = line;
      Column = column;
    }

    public override string ToString()
    {
      return string.Format("{0}: {1},{2}", InputName, Line, Column);
    }
  }

  public struct Token
  {
    public TokenKind Kind;
    public string Text;
    public TokenPosition Position;

    public Token(TokenKind kind, string text, TokenPosition pos)
    {
      Kind = kind;
      Text = text;
      Position = pos;
    }

    public override string ToString()
    {
      return string.Format("{0}: {1} {2}", Position.ToString(), Kind.ToString(), Text);
    }
  }
}
