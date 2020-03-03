using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Pianoforte.Solfege.Lang
{

  public class SyntaxTree
  {
    private static readonly Type blockType = typeof(Runtime.Block);
    private static readonly ConstructorInfo blockConstructor
      = blockType.GetConstructor(new[] { blockType });
    private static readonly ParameterExpression blockParam
      = Expression.Parameter(blockType, "block");



    private static Expression CallOperation(string name, params Expression[] args)
      => Expression.Call(
        null,
        typeof(Runtime.Operations).GetMethod(name),
        args);
    private static Expression CallOperation(string name, params AST[] args)
      => CallOperation(name, args.Select((v) => v.ToExpression()).ToArray());


    private static readonly Type valueType = typeof(Runtime.Value);
    private static Expression ValueProp(AST value, string prop)
      => ValueProp(value.ToExpression(), prop);
    private static Expression ValueProp(Expression value, string prop)
      => Expression.Property(value, valueType.GetProperty(prop));
    private static Expression MakeValue(string name, params object[] args)
      => MakeValue(name, args.Select((v) => Expression.Constant(v)));
    private static Expression MakeValue(string name, params Expression[] args)
      => Expression.Call(null, valueType.GetMethod("Make" + name), args);

    private static Expression AssignVar(string name, AST v)
      => AssignVar(name, v.ToExpression());
    private static Expression AssignVar(string name, Expression v)
      => Expression.Call(
          blockParam,
          blockType.GetMethod("SetVariable"),
          Expression.Constant(name),
          v
        );

    public abstract class AST
    {
      public AST Parent { get; set; }
      public Token Token { get; }
      public abstract Expression ToExpression();

      public AST(Token token)
      {
        Token = token;
      }

      public AST Lookup<T>()
      {
        var node = this;
        while ((node = node.Parent) != null)
        {
          if (node is T)
          {
            return node;
          }
        }
        return null;
      }
    }

    public class ASTList : AST, IEnumerable<AST>
    {
      private readonly List<AST> list;

      public ASTList(Token token) : this(token, Enumerable.Empty<AST>()) { }
      public ASTList(Token token, params AST[] items) : this(token, items as IEnumerable<AST>) { }

      public ASTList(Token token, IEnumerable<AST> items) : base(token)
      {
        list = items.ToList();
        foreach (var v in list)
        {
          v.Parent = this;
        }
      }

      public AST this[int i]
      {
        get => list[i];
        set => list[i] = value;
      }

      public IEnumerator<AST> GetEnumerator()
        => list.GetEnumerator();
      IEnumerator IEnumerable.GetEnumerator()
        => list.GetEnumerator();

      public override Expression ToExpression()
        => Expression.Block(list.Select((v) => v.ToExpression()));
    }

    public class Binary : AST
    {
      public AST Lhs { get; }
      public AST Rhs { get; }

      public Binary(Token token, AST lhs, AST rhs) : base(token)
      {
        Lhs = lhs;
        Rhs = rhs;

        Lhs.Parent = this;
        Lhs.Parent = this;
      }

      public override Expression ToExpression()
      {
        var method = Token.Kind switch
        {
          TokenKind.OpPlus => "Add",
          TokenKind.OpMinus => "Subtract",
          TokenKind.OpMultiplication => "Multiple",
          TokenKind.OpDivision => "Devide",
          TokenKind.OpEquals => "Eq",
          TokenKind.OpNotEquals => "NotEq",
          TokenKind.OpLessThan => "LessThan",
          TokenKind.OpLessThanOrEquals => "LessThanOrEquals",
          TokenKind.OpGreaterThan => "GreaterThan",
          TokenKind.OpGreaterThanOrEquals => "GreaterThanOrEquals",
          _ => throw new InternalAssertionException("Binary operator is required"),

        };
        return CallOperation(method, Lhs, Rhs);
      }
    }

    public class Unary : AST
    {
      public AST Value { get; }

      public Unary(Token token, AST value) : base(token)
      {
        Value = value;
        Value.Parent = this;
      }

      public override Expression ToExpression()
      {
        var method = Token.Kind switch
        {
          TokenKind.Not => "Not",
          _ => throw new InternalAssertionException("Unary operator is required"),

        };
        return CallOperation(method, Value);
      }
    }

    public class Literal : AST
    {
      public Literal(Token token) : base(token) { }

      public override Expression ToExpression()
        => Expression.Constant(Runtime.Value.FromToken(Token));
    }

    public class Identifier : AST
    {
      public Identifier(Token token) : base(token) { }

      public override Expression ToExpression()
        => Expression.Constant(Token.Text);
    }

    public class Variable : AST
    {
      public string Name => Token.Text;

      public Variable(Token token) : base(token) { }

      public override Expression ToExpression()
        => Expression.Call(
          blockParam,
          blockType.GetMethod("GetVariable"),
          Expression.Constant(Name)
        );
    }


    public class Assign : AST
    {
      public string Name => Token.Text;
      public AST Expr { get; }

      public Assign(Token token, AST expr) : base(token)
      {
        Expr = expr;
        Expr.Parent = this;
      }

      public override Expression ToExpression()
        => AssignVar(Name, Expr);
    }

    public class MemberAccess : AST
    {
      public AST Receiver { get; }
      public string Name { get; }

      public MemberAccess(Token token, AST receiver, string name) : base(token)
      {
        Receiver = receiver;
        Name = name;

        Receiver.Parent = this;
      }

      public override Expression ToExpression()
        => CallOperation("MemberAccess", Receiver.ToExpression(), Expression.Constant(Name));
    }

    public class FunctionCall : AST
    {
      public AST Func { get; }
      public ASTList Args { get; }

      public FunctionCall(Token token, AST func) : this(token, func, new ASTList(token)) { }
      public FunctionCall(Token token, AST func, IEnumerable<AST> args) : this(token, func, new ASTList(token, args)) { }

      public FunctionCall(Token token, AST func, ASTList args) : base(token)
      {
        Func = func;
        Args = args;

        Func.Parent = this;
        Args.Parent = this;
      }

      public override Expression ToExpression()
        => CallOperation(
          "FunctionCall",
          Func.ToExpression(),
          Expression.NewArrayInit(valueType, Args.Select((v) => v.ToExpression())));
    }

    public class InitArray : AST
    {
      public ASTList Items { get; }

      public InitArray(Token token) : this(token, new ASTList(token)) { }

      public InitArray(Token token, IEnumerable<AST> items) : this(token, new ASTList(token, items)) { }

      public InitArray(Token token, ASTList items) : base(token)
      {
        Items = items;

        Items.Parent = this;
      }


      public override Expression ToExpression()
        => CallOperation(
          "InitArray",
          Expression.NewArrayInit(valueType, Items.Select((v) => v.ToExpression())));
    }

    public class InitArrayByTo : AST
    {
      public AST From { get; }
      public AST To { get; }
      public AST Step { get; }

      public InitArrayByTo(Token token, AST from, AST to, AST step) : base(token)
      {
        From = from;
        To = to;
        Step = step;
      }

      public override Expression ToExpression()
        => CallOperation(
          "InitArrayByTo",
          From.ToExpression(),
          To.ToExpression(),
          Step != null ? Step.ToExpression() : Expression.Constant(Runtime.Value.MakeNone()));
    }

    public class ArrayItem : AST
    {
      public AST Array { get; }
      public AST Index { get; }

      public ArrayItem(Token token, AST array, AST index) : base(token)
      {
        Array = array;
        Index = index;

        Array.Parent = this;
        Index.Parent = this;
      }

      public override Expression ToExpression()
        => CallOperation("ArrayItem", Array, Index);
    }

    public class AssignArrayItem : AST
    {
      public AST Array { get; }
      public AST Index { get; }

      public AST Value { get; }

      public AssignArrayItem(Token token, AST array, AST index, AST value) : base(token)
      {
        Array = array;
        Index = index;
        Value = value;

        Array.Parent = this;
        Index.Parent = this;
        Value.Parent = this;
      }

      public override Expression ToExpression()
        => CallOperation("SetArrayItem", Array, Index, Value);
    }

    public class If : AST
    {
      private AST test;
      public AST Test
      {
        get => test;
        set
        {
          test = value;
          if (test != null)
          {
            test.Parent = this;
          }
        }
      }
      private AST ifTrue;
      public AST IfTrue
      {
        get => ifTrue;
        set
        {
          ifTrue = value;
          if (ifTrue != null)
          {
            ifTrue.Parent = this;
          }
        }
      }
      private AST ifFalse;
      public AST IfFalse
      {
        get => ifFalse;
        set
        {
          ifFalse = value;
          if (ifFalse != null)
          {
            ifFalse.Parent = this;
          }
        }
      }

      public If(Token token, AST test, AST ifTrue, AST ifFalse) : base(token)
      {
        Test = test;
        IfTrue = ifTrue;
        IfFalse = ifFalse;
      }

      private Expression ToBoolExpr(AST v)
        => ValueProp(v, "Bool");

      public override Expression ToExpression()
        => ifFalse == null
          ? Expression.IfThen(ToBoolExpr(Test), IfTrue.ToExpression())
          : Expression.IfThenElse(ToBoolExpr(Test), IfTrue.ToExpression(), IfFalse.ToExpression());
    }

    public class For : AST
    {
      public AST Condition { get; }
      public Block Block { get; }

      public LabelTarget BreakTarget { get; } = Expression.Label();
      public LabelTarget ContinueTarget { get; } = Expression.Label();

      public For(Token token, AST condition, Block block) : base(token)
      {
        Condition = condition;
        Block = block;
      }
      public override Expression ToExpression() =>
        Expression.Block(
          Expression.Loop(
            Expression.Block(
              Block.ToExpression(),
              Expression.IfThen(Expression.Not(ValueProp(Condition, "Bool")),
                Expression.Break(BreakTarget)
              )
            ), BreakTarget, ContinueTarget
          )
        );
    }
    public class ForEach : AST
    {
      public string VarName { get; }
      public AST List { get; }
      public Block Block { get; }
      public LabelTarget BreakTarget { get; } = Expression.Label();
      public LabelTarget ContinueTarget { get; } = Expression.Label();

      public ForEach(Token token, Token variable, AST list, Block block) : base(token)
      {
        VarName = variable.Text;
        List = list;
        Block = block;
      }

      public override Expression ToExpression()
      {
        var i = Expression.Parameter(typeof(int), "i");
        var len = ValueProp(CallOperation("Length", List), "Integer");
        var getItem = CallOperation("ArrayItem", List.ToExpression(), MakeValue("Integer", i));
        return Expression.Block(
            new[] { i },
          Expression.Loop(

          Expression.Block(
            AssignVar(VarName, getItem),

            Block.ToExpression(),

            Expression.AddAssign(i, Expression.Constant(1)),
            Expression.IfThen(Expression.GreaterThanOrEqual(i, len),
              Expression.Break(BreakTarget)
            )
          ), BreakTarget, ContinueTarget));
      }
    }

    public class Block : AST
    {
      private readonly ParameterExpression parentParam = Expression.Parameter(blockType, "parent");

      public ASTList Body { get; }

      public Block(Token token, IEnumerable<AST> body) : this(token, new ASTList(token, body)) { }
      public Block(Token token, ASTList body) : base(token)
      {
        Body = body;
        Body.Parent = this;
      }

      public Block(Token token, Block inner) : this(token, new ASTList(token, inner)) { }

      public override Expression ToExpression()
      {
        var passign =
           Expression.Assign(parentParam, blockParam);
        var bassign =
           Expression.Assign(blockParam, Expression.New(blockConstructor, parentParam));
        return Expression.Block(
           new[] { parentParam },
           passign,
           bassign,
           Body.ToExpression()
        );
      }
      /*
        => Expression.Block(
          new[] { parentParam },
          Expression.Assign(parentParam, EblockParam),
          Expression.Assign(blockParam, Expression.New(blockConstructor, parentParam)),
          Expression.Block(new[] { blockParam }, Body.ToExpression())
        );
        */
    }

    public class RootBlock : Block
    {
      private readonly Runtime.Block block;

      public RootBlock(Token token, Runtime.Block block, IEnumerable<AST> body) : this(token, block, new ASTList(token, body)) { }

      public RootBlock(Token token, Runtime.Block block, ASTList body) : base(token, body)
      {
        this.block = block;
      }

      public Delegate Compile()
        => (ToExpression() as LambdaExpression).Compile();

      public override Expression ToExpression()
        => Expression.Lambda(
          Expression.Block(
            new[] { blockParam },
            new[] {
              Expression.Assign(blockParam, Expression.Constant(block)),
              Body.ToExpression(),
            }
          )
        );
    }
  }
}
