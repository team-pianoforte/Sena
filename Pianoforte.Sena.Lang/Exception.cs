using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang
{
  public class InternalException : Exception
  {

  }

  public class SyntaxException : Exception
  {
    public TokenPosition Position { get; }
    public SyntaxException(TokenPosition pos, string message) : base($"SyntaxError at {pos}: {message}")
    {
      this.Position = pos;
    }
  }

  public class RuntimeException : Exception
  {
  }
}
