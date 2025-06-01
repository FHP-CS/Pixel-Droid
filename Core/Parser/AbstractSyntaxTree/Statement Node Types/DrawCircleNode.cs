
// Spawn(x, y)
public class  DrawCircleNode : StatementNode
{
    public ExpressionNode Radius { get; }


    public DrawCircleNode(ExpressionNode radius)
    {
        Radius = radius;
    }
    public override string ToString() => $"DrawCircle({Radius})";
}