using System;
using System.Linq.Expressions;
using System.IO;

namespace Pianoforte.Sena.Lang
{
  public class Engine
  {
    public Runtime.Environment Environment { get; }
    public Engine(Runtime.Environment env)
    {
      Environment = env;
    }
    public void Execute(string filename)
      => Execute(filename, new FileStream(filename, FileMode.Open));


    public void Execute(string filename, Stream input)
    {
      try
      {
        var f = Compile(filename, input);
        f.DynamicInvoke();
      }
      catch (SenaUserException e)
      {
        Console.WriteLine(e.ToString());
      }
    }

    public Delegate Compile(string filename)
      => Compile(filename, new FileStream(filename, FileMode.Open));

    public Delegate Compile(string filename, Stream input)
    {
      var parser = new Parser(new Lexer(filename, input));
      var expr = parser.Parse(Environment);
      return expr.Compile();
    }
  }
}
