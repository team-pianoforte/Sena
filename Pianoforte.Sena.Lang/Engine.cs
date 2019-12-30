using System;
using System.Linq.Expressions;
using System.IO;

namespace Pianoforte.Sena.Lang
{
  public class Engine
  {
    public void Execute(string filename)
      => Execute(filename, new FileStream(filename, FileMode.Open));
     

    public void Execute(string filename, Stream input)
    {
      var f = Compile(filename, input);
      f.DynamicInvoke();
    }

    public Delegate Compile(string filename)
      => Compile(filename, new FileStream(filename, FileMode.Open));

    public Delegate Compile(string filename, Stream input)
    {
      var parser = new Parser(new Lexer(filename, input));
      var expr = parser.Parse();
      return expr.Compile();
    }
  }
}
