using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class  DrawLineNode : StatementNode
{
    public ExpressionNode dirX { get; }
    public ExpressionNode dirY { get; }
    public ExpressionNode Distance { get; }


    public DrawLineNode(ExpressionNode dir_x, ExpressionNode dir_y, ExpressionNode _distance)
    {
        dirX = dir_x;
        dirY = dir_y;
        Distance = _distance;
    }
    public override string ToString() => $"DrawLine({dirX}, {dirY}, {Distance})";
}