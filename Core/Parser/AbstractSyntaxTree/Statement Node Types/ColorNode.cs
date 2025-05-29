// Color(colorString)
public class ColorNode : StatementNode
{
    public ExpressionNode ColorExpression { get; } // Could be a StringLiteralNode or a variable holding a color string

    public ColorNode(ExpressionNode colorExpression)
    {
        ColorExpression = colorExpression;
    }
    public override string ToString() => $"Color({ColorExpression})";
}