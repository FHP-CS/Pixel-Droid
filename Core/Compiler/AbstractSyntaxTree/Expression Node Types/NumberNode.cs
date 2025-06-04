using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class NumberNode : ExpressionNode
{
    public int Value { get; } // PDF specifies "Número entero" for literals in expressions

    public NumberNode(int value)
    {
        Value = value;
    }
    public override string ToString() => Value.ToString();
    public override object Evaluate(Interpreter Interpreter) => Value;
}