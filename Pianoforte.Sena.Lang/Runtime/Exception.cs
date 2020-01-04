using System;
using System.Collections.Generic;
using System.Text;

namespace Pianoforte.Sena.Lang.Runtime
{
  public class RuntimeException : Exception
  {
    public RuntimeException() { }
    public RuntimeException(string message) : base(message) { }
  }
}
