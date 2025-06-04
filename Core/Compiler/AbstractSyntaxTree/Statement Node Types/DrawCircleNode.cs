using System;
using PixelWallE.Common;
using PixelWallE.Execution;
// Spawn(x, y)
public class  DrawCircleNode : StatementNode
{
    public ExpressionNode Radius { get; }


    public DrawCircleNode(ExpressionNode radius)
    {
        Radius = radius;
    }
    public override string ToString() => $"DrawCircle({Radius})";
    public override void Execute(Interpreter interpreter)
    {
        throw new NotImplementedException();
    }
}