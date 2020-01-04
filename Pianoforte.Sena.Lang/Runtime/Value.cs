using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public enum ValueType
  {
    None,
    Bool,
    Number,
    String,
  }

  public struct Value
  {
    public const string NoneStr = "None";
    public const string TrueStr = "True";
    public const string FalseStr = "False";
    public ValueType Type { get; set; }

    private void AssertType(ValueType t)
    {
      if (Type != t)
      {
        throw new InternalAssertionException("Type mismatch");
      }
    }

    #region Properties

    private bool _bool;
    public bool Bool
    {
      get
      {
        AssertType(ValueType.Bool);
        return _bool;
      }
      set { _bool = value; }
    }

    private decimal _number;
    public decimal Number
    {
      get
      {
        AssertType(ValueType.Number);
        return _number;
      }
      set { _number = value; }
    }

    private string _string;
    public string String
    {
      get
      {
        AssertType(ValueType.String);
        return _string;
      }
      set { _string = value; }
    }

    #endregion

    #region Constructors
    public Value(ValueType type) : this(type, false, 0, "")
    {
    }

    public Value(ValueType type, bool b, decimal num, string str)
    {
      Type = type;
      _bool = b;
      _number = num;
      _string = str;
    }
    #endregion

    #region FromXXX

    public static Value FromObject(object value)
    {
      return value switch
      {
        bool b => MakeBool(b),
        decimal n => MakeNumber(n),
        string s => MakeString(s),
        null => MakeNone(),
        _ => throw new InternalAssertionException("Cannot convert RuntimeValue"),
      };
    }

    public static Value FromToken(Token token)
    {
      var type = token.Kind switch
      {
        TokenKind.NoneLiteral => ValueType.None,
        TokenKind.TrueLiteral => ValueType.Bool,
        TokenKind.FalseLiteral => ValueType.Bool,
        TokenKind.NumberLiteral => ValueType.Number,
        TokenKind.StringLiteral => ValueType.String,
        _ => throw new InternalAssertionException("Cannot convert RuntimeValue"),
      };
      return MakeString(token.Text).ConvertType(type);
    }

    #endregion

    #region MakeXXX
    public static Value MakeNone()
    {
      return new Value(ValueType.None);
    }

    public static Value MakeBool(bool v)
    {
      return new Value(ValueType.Bool, v, 0, "");
    }

    public static Value MakeNumber(decimal v)
    {
      return new Value(ValueType.Number, false, v, "");
    }

    public static Value MakeString(string v)
    {
      return new Value(ValueType.String, false, 0, v);
    }
    #endregion

    #region Converts

    public override string ToString()
    {
      return Type switch
      {
        ValueType.None => NoneStr,
        ValueType.Bool => Bool ? TrueStr : FalseStr,
        ValueType.Number => Number.ToString(),
        ValueType.String => String,
        _ => throw new InternalAssertionException("Unknown RuntimeValueType"),
      };
    }

    public Value ConvertType(ValueType type)
    {
      if(Type == type)
      {
        return this;
      }
      var s = ToString();
      if (type == ValueType.None)
      {
        return MakeNone();
      }
      else if (type == ValueType.Bool && (s == TrueStr || s == FalseStr))
      {
        return MakeBool(s == TrueStr);
      }
      else if (type == ValueType.Number)
      {
        if (decimal.TryParse(s, out var n))
        {
          return MakeNumber(n);
        }
      }
      else if (type == ValueType.String)
      {
        return MakeString(s);
      }
      throw new RuntimeException(string.Format(Properties.Resources.CannotConvertType, s, type));
    }

    #endregion

  }
}
