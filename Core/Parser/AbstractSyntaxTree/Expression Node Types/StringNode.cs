public class StringNode : ExpressionNode
{
    public string Value { get; }

    public StringNode(string value)
    {
        Value = value;
    }
    public override string ToString() => $"\"{Value}\"";
}