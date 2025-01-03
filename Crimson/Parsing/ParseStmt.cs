using Crimson.Lexing;

namespace Crimson.Parsing;

internal partial class Parser
{
    internal Stmt ParseExprStmt(int bindingPower)
    {
        Expr expr = ParseExpr(bindingPower);
        Advance();
        Expect(TokenKind.Semicolon, "");
        return new ExprStmt(expr);
    }
}