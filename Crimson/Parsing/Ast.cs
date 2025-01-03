﻿using Crimson.Lexing;

namespace Crimson.Parsing;

internal enum NodeKind
{
    NumLitExpr,
    BoolLitExpr,
    NilLitExpr,
    PrefixExpr,
    BinaryExpr,
    PrintStmt,
    PrintAstStmt,
}

internal abstract class Stmt
{
    internal abstract NodeKind StmtKind { get; }
}

internal abstract class Expr
{
    internal abstract NodeKind ExprKind { get; }
}

internal class NumLitExpr : Expr
{
    public double Value { get; set; }
    private Token _token;
    
    internal NumLitExpr(double value, Token token)
    {
        Value = value;
        _token = token;
    }

    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.NumLitExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(NumLitExpr\tValue: {0})", Value);
    }
}

internal class BoolLitExpr : Expr
{
    public bool Value { get; set; }
    private Token _token;
    
    internal BoolLitExpr(bool value, Token token)
    {
        Value = value;
        _token = token;
    }

    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.BoolLitExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(BoolLitExpr\tValue: {0})", Value);
    }
}

internal class NilLitExpr : Expr
{
    public string Value { get; set; }
    
    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.NilLitExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(Nil\tValue: {0})", Value);
    }
}

internal class PrefixExpr : Expr
{
    public TokenKind Op { get; set; }
    public Expr Expression { get; set; }

    internal PrefixExpr(TokenKind op, Expr expr)
    {
        Op = op;
        Expression = expr;
    }

    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.BinaryExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(PrefixExpr\tOp: {0}\tExpr: {1})", Op.ToString(), Expression.ToString());
    }
}

internal class GroupingExpr : Expr
{
    public Expr Expression { get; set; }

    internal GroupingExpr(Expr expr)
    {
        Expression = expr;
    }

    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.BinaryExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(GroupingExpr\tExpr: {0})", Expression.ToString());
    }
}

internal class BinaryExpr : Expr
{
    public TokenKind Op { get; set; }
    public Expr Left { get; set; }
    public Expr Right { get; set; }

    internal BinaryExpr(TokenKind op, Expr left, Expr right)
    {
        Op = op;
        Left = left;
        Right = right;
    }

    internal override NodeKind ExprKind
    {
        get
        {
            return NodeKind.BinaryExpr;
        }
    }

    public override string ToString()
    {
        return String.Format("(BinaryExpr:\n\tOp: {0}\tLeft: {1}\tRight: {2})", Op.ToString(), Left.ToString(), Right.ToString());
    }
}

internal class ExprStmt : Stmt
{
    public Expr Expression { get; set; }

    internal ExprStmt(Expr expression)
    {
        Expression = expression;
    }

    internal override NodeKind StmtKind
    {
        get
        {
            return Expression.ExprKind;
        }
    }

    public override string ToString()
    {
        return Expression.ToString() ?? String.Format("something");
    }
}

internal class PrintStmt : Stmt
{
    public Expr Expression { get; set; }

    internal PrintStmt(Expr expression)
    {
        Expression = expression;
    }

    internal override NodeKind StmtKind
    {
        get
        {
            return NodeKind.PrintStmt;
        }
    }

    public override string ToString()
    {
        return String.Format("(__Builtin: Print\tArg:\t{0})", Expression.ToString()) ?? String.Format("something");
    }
}

internal class PrintAstStmt : Stmt
{
    public Expr Expression { get; set; }

    internal PrintAstStmt(Expr expression)
    {
        Expression = expression;
    }

    internal override NodeKind StmtKind
    {
        get
        {
            return NodeKind.PrintAstStmt;
        }
    }

    public override string ToString()
    {
        return String.Format("(__Builtin: PrintAst\tArg:\t{0})", Expression.ToString()) ?? String.Format("something");
    }
}
