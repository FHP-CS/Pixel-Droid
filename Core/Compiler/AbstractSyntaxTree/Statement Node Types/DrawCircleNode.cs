using System;
using PixelWallE.Common;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay

// Spawn(x, y)
public class DrawCircleNode : StatementNode
{
    public ExpressionNode dirX { get; }
    public ExpressionNode dirY { get; }
    public override Token Token { get; }
    public ExpressionNode Radius { get; }


    public DrawCircleNode(ExpressionNode dirx, ExpressionNode diry, ExpressionNode radius, Token token)
    {
        dirX = dirx;
        dirY = diry;
        Radius = radius;
        Token = token;
    }
    public override string ToString() => $"DrawCircle({Radius})";
    public override async Task Execute(Interpreter interpreter)
    {
        object dirXobj = dirX.Evaluate(interpreter);
        object dirYobj = dirY.Evaluate(interpreter);
        object radius = Radius.Evaluate(interpreter);

        if (radius is int r && dirXobj is int dir_x && dirYobj is int dir_y)
        {
            if(!(ValidDir(dir_x) && ValidDir(dir_y)))        throw new RuntimeError($"Argument dirX , and dirY must represent a direction, 1, 0 or -1", Token);
            int x = interpreter.WallEInstance.X + dir_x*(r-1);
            int y = interpreter.WallEInstance.Y + dir_y*(r-1);//le reste 1 para que quede como el del pdf 
            interpreter.WallEInstance.Spawn(x,y);

            if (!await interpreter.WallEInstance.DrawCircle(r))
                throw new RuntimeError($"Invalid argument {r}", Token);
        }

        else
        {
            throw new RuntimeError($"Argument {radius} must be integer", Token);
        }
    }
    public bool ValidDir(int x)
    {
        return (x == 1 || x== 0 || x== -1) ? true: false;
    }
}