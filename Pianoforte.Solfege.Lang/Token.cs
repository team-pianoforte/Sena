using System;
using System.Collections.Generic;
using System.Linq;

namespace Pianoforte.Solfege.Lang
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

    Begin,
    If,
    Elif,
    Else,
    For,
    End,
    Func,

    And,
    Or,
    Not,
    To,
    Step,
    In,

    OpAssignment,
    OpEquals,
    OpNotEquals,
    OpLessThan,
    OpLessThanOrEquals,
    OpGreaterThan,
    OpGreaterThanOrEquals,
    OpPlus,
    OpMinus,
    OpMultiplication,
    OpDivision,
    ParenLeft,
    ParenRight,
    SquareBracketLeft,
    SquareBracketRight,
    Dot,
    DotDotDot,
    Comma,
  }

  public static class TokenExtensions
  {
    public static bool IsAssignment(this Token v) => v.Kind switch
    {
      TokenKind.OpAssignment => true,
      _ => false,
    };
    public static bool IsBoolBinaryOp(this Token v)
      => v.Kind == TokenKind.And
      || v.Kind == TokenKind.Or;

    public static bool IsBinaryOp(this Token v)
      => v.IsTermOp()
      || v.IsFactorOp()
      || v.IsComparsionOp()
      || v.IsBoolBinaryOp()
      || v.Kind == TokenKind.To;

    public static bool IsUnaryOp(this Token v)
      => v.Kind == TokenKind.Not;

    public static bool IsTermOp(this Token v) => v.Kind switch
    {
      TokenKind.OpPlus => true,
      TokenKind.OpMinus => true,
      _ => false,
    };

    public static bool IsFactorOp(this Token v) => v.Kind switch
    {
      TokenKind.OpMultiplication => true,
      TokenKind.OpDivision => true,
      _ => false,
    };

    public static bool IsComparsionOp(this Token v) => v.Kind switch
    {
      TokenKind.OpEquals => true,
      TokenKind.OpNotEquals => true,
      TokenKind.OpLessThan => true,
      TokenKind.OpLessThanOrEquals => true,
      TokenKind.OpGreaterThan => true,
      TokenKind.OpGreaterThanOrEquals => true,
      _ => false,
    };

    public static bool IsLiteral(this Token v) => v.Kind switch
    {
      TokenKind.NoneLiteral => true,
      TokenKind.TrueLiteral => true,
      TokenKind.FalseLiteral => true,
      TokenKind.NumberLiteral => true,
      TokenKind.StringLiteral => true,
      _ => false,
    };

    public static bool IsConstituteBlock(this Token v) => v.Kind switch
    {
      TokenKind.Begin => true,
      TokenKind.For => true,
      TokenKind.If => true,
      TokenKind.Func => true,
      _ => false,
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
    public static readonly Keyword Begin = new Keyword("begin", TokenKind.Begin);
    public static readonly Keyword If = new Keyword("if", TokenKind.If);
    public static readonly Keyword Elif = new Keyword("elif", TokenKind.Elif);
    public static readonly Keyword Else = new Keyword("else", TokenKind.Else);
    public static readonly Keyword For = new Keyword("for", TokenKind.For);
    public static readonly Keyword Func = new Keyword("func", TokenKind.Func);
    public static readonly Keyword End = new Keyword("end", TokenKind.End);
    public static readonly Keyword And = new Keyword("and", TokenKind.And);
    public static readonly Keyword Or = new Keyword("or", TokenKind.Or);
    public static readonly Keyword Not = new Keyword("not", TokenKind.Not);
    public static readonly Keyword To = new Keyword("to", TokenKind.To);
    public static readonly Keyword Step = new Keyword("step", TokenKind.Step);
    public static readonly Keyword In = new Keyword("in", TokenKind.In);

    public static readonly Dictionary<string, Keyword> Map = new Dictionary<string, Keyword> {
      { None.Text, None },
      { True.Text, True },
      { False.Text, False },
      { Begin.Text, Begin },
      { If.Text, If },
      { Elif.Text, Elif },
      { Else.Text, Else },
      { For.Text, For },
      { Func.Text, Func },
      { End.Text, End },
      { And.Text, And },
      { Or.Text, Or },
      { Not.Text, Not },
      { To.Text, To },
      { Step.Text, Step },
      { In.Text, In },
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
