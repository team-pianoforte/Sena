using System;
using System.IO;
using System.Text;

namespace Pianoforte.Sena.Cli
{
  class Program
  {
    static void Main(string[] args)
    {
      var engine = new Sena.Lang.Engine();
      Directory.GetCurrentDirectory();
      var filename = Path.Join("Examples", "Hello.sena");
      engine.Execute(filename);
    }
  }
}
