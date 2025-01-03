using Crimson.Lexing;

namespace Crimson.Parsing;

internal partial class Parser
{
    internal Stmt ParseStmt()
    {
        switch (_currToken.Kind)
        {
            case TokenKind.Print:
                return ParsePrintStmt();
            case TokenKind.PrintAst:
                return ParsePrintAstStmt();
            default:
                return ParseExprStmt((int)BindingPower.None);
        }
    }

    internal Stmt ParseExprStmt(int bindingPower)
    {
        Expr expr = ParseExpr(bindingPower);
        Advance();
        Expect(TokenKind.Semicolon, "");
        return new ExprStmt(expr);
    }

    internal Stmt ParsePrintStmt()
    {
        Advance();
        Expr expr = ParseExpr((int)BindingPower.None);
        Advance();
        Expect(TokenKind.Semicolon, "Print statement must end with a semicolon");
        return new PrintStmt(expr);
    }

    internal Stmt ParsePrintAstStmt()
    {
        Advance();
        Expr expr = ParseExpr((int)BindingPower.None);
        Advance();
        Expect(TokenKind.Semicolon, "PrintAst statement must end with a semicolon");
        return new PrintAstStmt(expr);
    }
}