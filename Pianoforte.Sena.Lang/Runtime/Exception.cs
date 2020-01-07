using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public class RuntimeException : SenaUserException
  {
    public RuntimeException() { }
    public RuntimeException(string message) : base(message) { }

    public override string ToString()
    {
      return string.Format(Properties.Resources.RuntimeError, Message);
    }
  }
}
