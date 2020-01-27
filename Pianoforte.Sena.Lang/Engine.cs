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

    private Delegate bin = null;

    public void Execute(string filename)
      => Execute(filename, new FileStream(filename, FileMode.Open));


    public void Execute(string filename, Stream input)
    {
      try
      {
        if (bin is null)
        {
          Compile(filename, input);
        }
        bin.DynamicInvoke();
      }
      catch (SenaUserException e)
      {
        Console.WriteLine(e.ToString());
      }
    }

    public void Compile(string filename)
      => Compile(filename, new FileStream(filename, FileMode.Open));

    public void Compile(string filename, Stream input)
    {
      var parser = new Parser(new Lexer(filename, input));
      var expr = parser.Parse(Environment);
      bin = expr.Compile();
    }
  }
}
