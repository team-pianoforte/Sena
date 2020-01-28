using System;
using Xunit;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class FunctionTest
  {
    static readonly ParameterExpression xParam = Expression.Parameter(typeof(Value), "x");

    private readonly Function lambdaFunction, funcFunction;
    private readonly SyntaxBuilder syntax;

    public FunctionTest() {
      lambdaFunction = new Function(Expression.Lambda(
        syntax.BinaryExpr(
          Expression.Constant(Value.MakeNumber(1), typeof(Value)),
          new Token(TokenKind.OpPlus, "+", new TokenPosition("", 1, 2)),
          xParam
        ), "f", new[] { xParam }
      ));
      funcFunction = new Function((args) => Operations.Add(Value.MakeNumber(1), args[0]), "f", "x");
    }
    [Fact]
    public void TestToString()
    {
      Assert.Equal("func f(x)", lambdaFunction.ToString());
      Assert.Equal("func f(x)", funcFunction.ToString());
    }

    [Fact]
    public void TestCall()
    {
      var x = Value.MakeNumber(10);
      Assert.Equal(Value.MakeNumber(11), lambdaFunction.Call(x));
      Assert.Equal(Value.MakeNumber(11), funcFunction.Call(x));
    }

    [Fact]
    public void TestCallWithInvalidNuberOfArgument()
    {
      Assert.Throws<RuntimeException>(() => lambdaFunction.Call());
      Assert.Throws<RuntimeException>(() => funcFunction.Call());
    }

    [Fact]
    public void TestReturnsInvalidValue()
    {
      var f = new Function(Expression.Lambda(Expression.Constant(0)));
      Assert.Throws<InternalAssertionException>(() => f.Call());
    }

    [Fact]
    public void TestFunctionThatReturnsNull()
    {
      var f = new Function(Expression.Lambda(Expression.Constant(null)));
      Assert.Equal(Value.MakeNone(), f.Call());
    }

    [Fact]
    public void TestVoidFunction()
    {
      var f = new Function(Expression.Lambda(Expression.Block()));
      Assert.Equal(Value.MakeNone(), f.Call());
    }
  }
}
