using Crimson.Lexing;

namespace Crimson.Parsing;

internal enum NodeKind
{
    NumLitExpr,
    PrefixExpr,
    BinaryExpr,
}

internal abstract class Stmt
{
    public abstract NodeKind StmtKind();
}

internal abstract class Expr
{
    public abstract NodeKind ExprKind();
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

    public override NodeKind ExprKind() { return NodeKind.NumLitExpr; }

    public override string ToString()
    {
        return String.Format("(NumLitExpr\tValue: {0})", Value);
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

    public override NodeKind ExprKind() { return NodeKind.PrefixExpr; }
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

    public override NodeKind ExprKind() { return NodeKind.BinaryExpr; }

    public override string ToString()
    {
        return String.Format("(BinaryExpr\tLeft: {0}\tRight: {1})", Left.ToString(), Right.ToString());
    }
}

internal class ExprStmt : Stmt
{
    public Expr Expression { get; set; }
    private Token _token;

    internal ExprStmt(Expr expression, Token token)
    {
        Expression = expression;
        _token = token;
    }

    public override NodeKind StmtKind() { return Expression.ExprKind(); }
}
