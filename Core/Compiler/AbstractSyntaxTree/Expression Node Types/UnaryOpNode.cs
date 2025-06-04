using System;
using PixelWallE.Common;
using PixelWallE.Execution;

public class UnaryOpNode : ExpressionNode
{
    Token Op { get; }
    ExpressionNode Expr { get; }
    public UnaryOpNode(Token op, ExpressionNode expr)
    {
        Op = op;
        Expr = expr;
    }
    public override string ToString()
    {
            return $"({Op.Literal} {Expr})";
    }
    public override object Evaluate(Interpreter interpreter)
    {
        return (-(int)Expr.Evaluate(interpreter));
    }
}