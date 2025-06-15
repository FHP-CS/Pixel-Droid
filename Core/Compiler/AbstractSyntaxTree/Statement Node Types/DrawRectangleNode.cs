// DrawRectangle(int width, int height)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class DrawRectangleNode : StatementNode
{
    public ExpressionNode dirX {get;}
    public ExpressionNode dirY {get;}
    public ExpressionNode distance {get;}

    public ExpressionNode Width { get; }
    public ExpressionNode Height { get; }
    public override Token Token {get; }


    public DrawRectangleNode(ExpressionNode dirx, ExpressionNode diry, ExpressionNode dist, ExpressionNode width, ExpressionNode height, Token token)
    {
        dirX = dirx;
        dirY = diry;
        distance = dist;
        Width = width;
        Height = height;
        Token = token;
    }
    public override string ToString() => $"DrawRectangle({Width}, {Height})";
    public override void Execute(Interpreter interpreter)
    {
        object dirXobj = dirX.Evaluate(interpreter);
        object dirYobj = dirY.Evaluate(interpreter);
        object distanceObj = distance.Evaluate(interpreter);
        object widthObj = Width.Evaluate(interpreter);
        object heightObj = Height.Evaluate(interpreter);


        if (dirXobj is int dir_x && dirYobj is int dir_y && distanceObj is int d && widthObj is int width && heightObj is int height)

        {
            if(d < 0)       throw new RuntimeError($"Argument distance, must be positive!", Token);
            if(!(ValidDir(dir_x) && ValidDir(dir_y)))        throw new RuntimeError($"Argument dirX , and dirY must represent a direction, 1, 0 or -1", Token);
            int x = interpreter.WallEInstance.X + dir_x*(d-1);
            int y = interpreter.WallEInstance.Y + dir_y*(d-1);//le reste 1 para que quede como el del pdf 
            interpreter.WallEInstance.Spawn(x,y);

            if (!interpreter.WallEInstance.DrawRectangle(dir_x,dir_y, d, width, height))
                throw new RuntimeError($"Invalid arguments ", Token);
        }

        else
        {
            throw new RuntimeError($"Arguments must be integers", Token);
        }
    }
    public bool ValidDir(int x)
    {
        return (x == 1 || x== 0 || x== -1) ? true: false;
    }
        throw new NotImplementedException();
    }
}