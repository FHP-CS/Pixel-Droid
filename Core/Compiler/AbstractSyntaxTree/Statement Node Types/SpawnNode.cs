
// Spawn(x, y)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class SpawnNode : StatementNode
{
    public Token SpawnToken { get; }
    public ExpressionNode X { get; }
    public ExpressionNode Y { get; }

    public SpawnNode(Token spawnToken, ExpressionNode x, ExpressionNode y)
    {
        SpawnToken = spawnToken;
        X = x;
        Y = y;
    }
    public override string ToString() => $"Spawn({X}, {Y})";
    public override void Execute(Interpreter Interpreter)
    {

        if (X.Evaluate(Interpreter) is int x && Y.Evaluate(Interpreter) is int y)
        {
            if (!Interpreter.WallEInstance.Spawn(x, y))
            {
                // As per PDF, runtime error stops execution
                throw new RuntimeError($"Spawn position ({X}, {Y}) is outside canvas bounds (W:{Interpreter.Canvas.Width}, H:{Interpreter.Canvas.Height}).", SpawnToken);
            }
        }
        else   throw new RuntimeError($"Arguments ({X} and {Y}) must be integers", SpawnToken);
        

    }
}