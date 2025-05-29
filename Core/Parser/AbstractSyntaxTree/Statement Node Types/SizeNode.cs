// Size(k)
public class SizeNode : StatementNode
{
    public ExpressionNode SizeExpression { get; }

    public SizeNode(ExpressionNode sizeExpression)
    {
        SizeExpression = sizeExpression;
    }
    public override string ToString() => $"Size({SizeExpression})";
}