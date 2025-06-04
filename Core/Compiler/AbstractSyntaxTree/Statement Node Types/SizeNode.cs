// Size(k)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class SizeNode : StatementNode
{
    public ExpressionNode SizeExpression { get; }
    public Token SizeToken;
    public SizeNode(Token sizeToken, ExpressionNode sizeExpression)
    {
        SizeToken = sizeToken;
        SizeExpression = sizeExpression;
    }
    public override string ToString() => $"Size({SizeExpression})";
    public override void Execute(Interpreter Interpreter)
    {

        if (SizeExpression.Evaluate(Interpreter) is int size)
            Interpreter.WallEInstance.SetSize(size);
        else
            throw new RuntimeError($"Argument ({SizeExpression} must be integers", SizeToken);


    }
}