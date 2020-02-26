using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Pianoforte.Solfege.Lang.Runtime
{
  internal class DesktopRuntime
  {
    internal class LibConsole : Library.ILibConsole
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
        Console.WriteLine(v);
      }

      public Value ReadLine()
      {
        PrepareIfRequired();
        return Value.MakeString(Console.ReadLine());
      }
    }
    internal class LibConvert : Library.ILibConvert { }

    internal class LibSystem : Library.ILibSystem
    {
      public Value Update()
      {
        return Value.MakeBool(true);
      }
    }

    public static Environment Environment = new Environment(new Library(
      new LibConsole(), new LibConvert(), new LibSystem()));
  }
}
