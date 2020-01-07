using System;
using Xunit;
using System.Linq.Expressions;

namespace Pianoforte.Sena.Lang.Runtime.Test
{
  public class FunctionTest
  {
    static readonly ParameterExpression xParam = Expression.Parameter(typeof(Value), "x");

    static readonly LambdaExpression expr = Expression.Lambda(
      Syntax.BinaryExpr(
        Expression.Constant(Value.MakeNumber(1), typeof(Value)),
        new Token(TokenKind.OpPlus, "+", new TokenPosition("", 1, 2)),
        xParam
      ), "f", new[] { xParam }
    );

    [Fact]
    public void TestToString()
    {
      var f = new Function(expr);
      Assert.Equal("func f(x)", f.ToString());
    }

    [Fact]
    public void TestCall()
    {
      var f = new Function(expr);
      var x = Value.MakeNumber(10);
      var res = f.Call(x);
      Assert.Equal(Value.MakeNumber(11), res);
    }

    [Fact]
    public void TestCallWithInvalidNuberOfArgument()
    {
      var f = new Function(expr);
      Assert.Throws<RuntimeException>(() => f.Call());
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
