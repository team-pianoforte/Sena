using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace Pianoforte.Sena.Lang
{
  public static class Syntax
  {
    private static readonly Type blockType = typeof(Runtime.Block);
    private static readonly ConstructorInfo blockConstructor
      = blockType.GetConstructor(new[] { blockType });
    private static readonly ParameterExpression blockParam
      = Expression.Parameter(blockType, "block");


    public static Expression Literal(Token token)
    {
      return Expression.Constant(Runtime.Value.FromToken(token));
    }

    public static Expression BinaryExpr(Expression lhs, Token op, Expression rhs)
    {
      if (!op.IsBinaryOp())
      {
      }
      var method = op.Kind switch
      {
        TokenKind.OpPlus => "Add",
        TokenKind.OpMinus => "Subtract",
        TokenKind.OpMultiplication => "Multiple",
        TokenKind.OpDivision => "Devide",
        _ => throw new InternalAssertionException("Binary operator is required"),

      };
      return Expression.Call(null, typeof(Runtime.Operations).GetMethod(method), lhs, rhs);
    }

    public static LambdaExpression Root(Runtime.Environment env, IEnumerable<Expression> lines)
      => Expression.Lambda(
        Expression.Block(
          new[] { blockParam },
          Enumerable.Concat(new [] {
            Expression.Assign(blockParam, Expression.Constant(env.RootBlock)),
          }, lines)));

    public static Expression Block(params Expression[] lines)
      => Block(lines);

    public static Expression Block(IEnumerable<Expression> lines)
      => Block(Enumerable.Empty<ParameterExpression>(), lines);

    public static Expression Block(IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> lines)
    {
      /*
       * {
       *   var parent = block;
       *   {
       *      var block = new Block(parent);
       *      ...
       *   }
       * }
       */
      var parentParam = Expression.Parameter(blockType, "parent");
      return Expression.Block(
        new[] { parentParam },
        Expression.Assign(parentParam, Expression.Constant(blockParam, blockType)),
        Expression.Block(
          Enumerable.Concat(parameters, new[] { blockParam }),
          Enumerable.Concat(
            new[] { Expression.Assign(blockParam, Expression.New(blockConstructor, parentParam)) },
            lines
          )
        )
      );
    }

    public static Expression Variable(string name)
    {
      var m = blockType.GetMethod("GetVariable");
      return Expression.Call(blockParam, m, Expression.Constant(name));
    }

    public static Expression Assign(string name, Expression expr)
    {
      var m = blockType.GetMethod("SetVariable");
      return Expression.Call(blockParam, m, Expression.Constant(name), expr);
    }

    public static Expression MemberAccess(Expression receiver, string name)
    {
      var method = typeof(Runtime.Operations).GetMethod("MemberAccess");
      return Expression.Call(null, method, receiver, Expression.Constant(name));
    }

    public static Expression FunctionCall(Expression func, params Expression[] args)
    {
      var method = typeof(Runtime.Operations).GetMethod("FunctionCall");
      return Expression.Call(null, method, func, Expression.NewArrayInit(typeof(Runtime.Value), args));
    }

    public static Expression InitArray(IEnumerable<Expression> items)
    {
      var method = typeof(Runtime.Operations).GetMethod("InitArray");
      return Expression.Call(null, method, Expression.NewArrayInit(typeof(Runtime.Value), items));
    }

    public static Expression ArrayItem(Expression arr, Expression i)
    {
      var method = typeof(Runtime.Operations).GetMethod("ArrayItem");
      return Expression.Call(null, method, arr, i);
    }

    public static Expression AssignArrayItem(Expression arr, Expression i, Expression v)
    {
      var method = typeof(Runtime.Operations).GetMethod("SetArrayItem");
      return Expression.Call(null, method, arr, i, v);
    }
  }
}
