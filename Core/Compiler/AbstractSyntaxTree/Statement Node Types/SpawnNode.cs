
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
        object Xobj = X.Evaluate(Interpreter);
        object Yobj = Y.Evaluate(Interpreter);
        
        if(!(Xobj is int x))    throw new RuntimeError($"X argument for Spawn must be an integer.", SpawnToken);
        if(!(Yobj is int y))    throw new RuntimeError($"Y argument for Spawn must be an integer.", SpawnToken);
        
        bool sucess = Interpreter.WallEInstance.Spawn(x,y);
        if(!sucess)
           throw new RuntimeError($"Spawn command failed. Position ({Xobj},{Yobj}) might be out of canvas bounds.", SpawnToken);
        

    }
}