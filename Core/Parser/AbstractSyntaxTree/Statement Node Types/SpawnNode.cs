
// Spawn(x, y)
public class SpawnNode : StatementNode
{
    public ExpressionNode X { get; }
    public ExpressionNode Y { get; }

    public SpawnNode(ExpressionNode x, ExpressionNode y)
    {
        X = x;
        Y = y;
    }
    public override string ToString() => $"Spawn({X}, {Y})";
}