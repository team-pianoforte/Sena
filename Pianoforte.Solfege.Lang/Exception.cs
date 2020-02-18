using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Solfege.Lang
{
  /// <summary>
  /// The exception that is throw when detect a abnormal behavior in language processor. Probably it is a bug.
  /// </summary>
  public class InternalAssertionException : Exception
  {
    public InternalAssertionException() : base() { }
    public InternalAssertionException(string message) : base(message) { }
    public InternalAssertionException(string message, Exception innerException) : base(message, innerException) { }

  }

  /// <summary>
  /// The exception that casued by sena user
  /// </summary>
  public class SenaUserException : Exception
  {
    public SenaUserException() : base() { }
    public SenaUserException(string message) : base(message) { }
  }

  /// <summary>
  /// The exception that is throw when found a parisng error.
  /// </summary>
  public class SyntaxException : SenaUserException
  {
    /// <summary>
    /// Position of syntax error.
    /// </summary>
    public TokenPosition Position { get; }
    /// <summary>
    /// The token that casused the exception.
    /// </summary>
    public Token? Token { get; }

    public SyntaxException(Token token, string message) : base(message)
    {
      this.Token = token;
      this.Position = token.Position;
    }

    public SyntaxException(TokenPosition pos, string message) : base(message)
    {
      this.Token = null;
      this.Position = pos;
    }

    public override string ToString()
    {
      return string.Format(Properties.Resources.SyntaxError, Position, Message);
    }
  }
}
