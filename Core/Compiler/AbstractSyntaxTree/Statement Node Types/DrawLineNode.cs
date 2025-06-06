using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class DrawLineNode : StatementNode
{
    public ExpressionNode dirX { get; }
    public ExpressionNode dirY { get; }
    public ExpressionNode Distance { get; }
    public Token Token { get; }


    public DrawLineNode(ExpressionNode dir_x, ExpressionNode dir_y, ExpressionNode _distance, Token token)
    {
        dirX = dir_x;
        dirY = dir_y;
        Distance = _distance;
        Token = token;
    }
    public override string ToString() => $"DrawLine({dirX}, {dirY}, {Distance})";
    public override void Execute(Interpreter interpreter)
    {
        object Dx = dirX.Evaluate(interpreter);
        object Dy = dirY.Evaluate(interpreter);
        object dist = Distance.Evaluate(interpreter);


        if (Dx is int dX && Dy is int dY && dist is int dS)
        {
            if (!interpreter.WallEInstance.DrawLine(dX, dY, dS))
            {
        throw new RuntimeError($"Invalid arguments {dirX} , {dirY} , {Distance}",Token);
            
            }
        }
        else 
          throw new RuntimeError($"Argument {dirX} , {dirY} , {Distance} must be integers", Token);
    }
}