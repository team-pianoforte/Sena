using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;


namespace Pianoforte.Solfege.Lang.Runtime
{
  internal static class CliRuntime
  {
    internal class Console : Library.IConsole
    {
      public void WriteLine(Value v)
      {
        System.Console.WriteLine(v);
      }
    }

    public static Environment Environment = new Environment(new Library(new Console()));
  }
}
