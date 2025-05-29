// For variables when used in an expression (e.g., the 'n' in k <- n * 2)
public class VariableNode : ExpressionNode
{
    public string Name { get; }

    public VariableNode(string name)
    {
        Name = name;
    }
    public override string ToString() => Name;
}