using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Pianoforte.Solfege.Lang.Runtime
{
  public class Library
  {
    private static Value MakeVoidFunc(Action<FunctionArgs> f, string name, params string[] argNames)
      => Value.MakeFunction(
        new Function((args) =>
        {
          f(args);
          return Value.MakeNone();
        }, name, argNames));

    private static Value MakeFunc(Func<FunctionArgs, Value> f, string name, params string[] argNames)
      => Value.MakeFunction(
        new Function(f, name, argNames));

    public ILibConsole LibConsole { get; }
    public interface ILibConsole
    {
      void WriteLine(Value v);
      Value ReadLine();

      public Object AsObject()
        => new Object("Console", new Dictionary<string, Value>() {
          { "WriteLine", MakeVoidFunc((args) => WriteLine(args[0]), "WriteLine", "v") },
          { "ReadLine", MakeFunc((args) => ReadLine(), "ReadLine") },
        });
    }
    public interface ILibDebug
    {
      void Error(Value v);

      public Object AsObject()
        => new Object("Debug", new Dictionary<string, Value>() {
          { "Error", MakeVoidFunc((args) => Error(args[0]), "Error", "v") },
        });
    }

    public ILibConvert LibConvert;
    public interface ILibConvert
    {
      Value ToString(Value v)
        => v.ConvertType(ValueType.String);
      Value ToBool(Value v)
        => v.ConvertType(ValueType.Bool);
      Value ToNumber(Value v)
        => v.ConvertType(ValueType.Number);

      public Object AsObject()
       => new Object("Convert", new Dictionary<string, Value>() {
          { "ToString", MakeFunc((args) => ToString(args[0]), "ToString", "v") },
          { "ToBool", MakeFunc((args) => ToBool(args[0]), "ToBool", "v") },
          { "ToNumber", MakeFunc((args) => ToNumber(args[0]), "ToNumber", "v") },
       });
    }
    public ILibSystem LibSystem;
    public interface ILibSystem
    {
      Value Update();

      public Object AsObject()
       => new Object("System", new Dictionary<string, Value>() {
          { "Update", MakeFunc((_) => Update(), "Update") },
       });
    }
    public IEnumerable<Object> Objects
    {
      get => new List<Object> {
        LibConsole.AsObject(),
        LibConvert.AsObject(),
        LibSystem.AsObject(),
      };
    }

    public Library(ILibConsole console, ILibConvert conv, ILibSystem system)
    {
      LibConsole = console;
      LibConvert = conv;
      LibSystem = system;
    }
  }
}
