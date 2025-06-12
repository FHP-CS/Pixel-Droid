// DrawRectangle(int width, int height)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class DrawRectangleNode : StatementNode
{
    public ExpressionNode Width { get; }
    public ExpressionNode Height { get; }
    public override Token Token {get; }


    public DrawRectangleNode(ExpressionNode width, ExpressionNode height, Token token)
    {
        Width = width;
        Height = height;
        Token = token;
    }
    public override string ToString() => $"DrawRectangle({Width}, {Height})";
    public override void Execute(Interpreter interpreter)
    {
        throw new NotImplementedException();
    }
}