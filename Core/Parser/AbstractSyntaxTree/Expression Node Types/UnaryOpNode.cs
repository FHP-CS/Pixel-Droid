using PixelWallE.Common;

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
}