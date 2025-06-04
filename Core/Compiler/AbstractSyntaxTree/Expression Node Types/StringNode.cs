using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class StringNode : ExpressionNode
{
    public string Value { get; }

    public StringNode(string value)
    {
        Value = value;
    }
    public override object Evaluate(Interpreter Interpreter) => Value;

    public override string ToString() => $"\"{Value}\"";
}