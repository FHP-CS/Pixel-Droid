// Size(k)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay
public class SizeNode : StatementNode
{
    public ExpressionNode SizeExpression { get; }
    public override Token Token { get; }

    public SizeNode(Token token, ExpressionNode sizeExpression)
    {
        Token = token;
        SizeExpression = sizeExpression;
    }
    public override string ToString() => $"Size({SizeExpression})";
    public override Task Execute(Interpreter Interpreter)
    {

        if (SizeExpression.Evaluate(Interpreter) is int size)
            Interpreter.WallEInstance.SetSize(size);
        else
            throw new RuntimeError($"Argument ({SizeExpression} must be integers", Token);
        return Task.CompletedTask;
    }
}