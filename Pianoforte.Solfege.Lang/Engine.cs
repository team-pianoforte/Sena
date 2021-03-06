﻿using System;
using System.Linq.Expressions;
using System.IO;
using System.Threading.Tasks;

namespace Pianoforte.Solfege.Lang
{
  public class Engine
  {
    public Runtime.Environment Environment { get; }
    public Engine(Runtime.Environment env)
    {
      Environment = env;
    }

    private Delegate bin = null;

    public async void ExecuteAsync(string filename)
      => await Task.Run(() => Execute(filename));

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
      var ast = parser.Parse(Environment);
      bin = ast.Compile();
    }
  }
}
