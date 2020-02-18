using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Pianoforte.Solfege.Lang.Runtime
{
  internal class DesktopRuntime
  {
    internal class Console : Library.IConsole
    {
      [DllImport("kernel32.dll")]
      private static extern bool AllocConsole();

      private bool allocked = false;

      private void PrepareIfRequired()
      {
        if (!allocked)
        {
          AllocConsole();
          allocked = true;
        }
      }

      public void WriteLine(Value v)
      {
        PrepareIfRequired();
        System.Console.WriteLine(v);
      }
    }

    public static Environment Environment = new Environment(new Library(new Console()));
  }
}
