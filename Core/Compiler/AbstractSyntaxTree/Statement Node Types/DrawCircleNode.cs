using System;
using PixelWallE.Common;
using PixelWallE.Execution;
// Spawn(x, y)
public class  DrawCircleNode : StatementNode
{
    public override Token Token { get; }
    public ExpressionNode Radius { get; }


    public DrawCircleNode(ExpressionNode radius, Token token)
    {
        Radius = radius;
        Token = token;
    }
    public override string ToString() => $"DrawCircle({Radius})";
    public override void Execute(Interpreter interpreter)
    {
        throw new NotImplementedException();
    }
}