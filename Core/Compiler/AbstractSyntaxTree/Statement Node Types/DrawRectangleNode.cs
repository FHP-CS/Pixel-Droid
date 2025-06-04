// DrawRectangle(int width, int height)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class DrawRectangleNode : StatementNode
{
    public ExpressionNode Width { get; }
    public ExpressionNode Height { get; }

    public DrawRectangleNode(ExpressionNode width, ExpressionNode height)
    {
        Width = width;
        Height = height;
    }
    public override string ToString() => $"DrawRectangle({Width}, {Height})";
    public override void Execute(Interpreter interpreter)
    {
        throw new NotImplementedException();
    }
}