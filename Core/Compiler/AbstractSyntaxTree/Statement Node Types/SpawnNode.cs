
// Spawn(x, y)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class SpawnNode : StatementNode
{
    public override Token Token { get; }
    public ExpressionNode X { get; }
    public ExpressionNode Y { get; }

    public SpawnNode(Token token, ExpressionNode x, ExpressionNode y)
    {
        Token = token;
        X = x;
        Y = y;
    }
    public override string ToString() => $"Spawn({X}, {Y})";
    public override void Execute(Interpreter Interpreter)
    {
        // if(Interpreter._isSpawned) throw new RuntimeError($"WallE is already spawned! What are you doing mate? :()", Token);
        object Xobj = X.Evaluate(Interpreter);
        object Yobj = Y.Evaluate(Interpreter);
        
        if(!(Xobj is int x))    throw new RuntimeError($"X argument for Spawn must be an integer but got {Xobj}.", Token);
        if(!(Yobj is int y))    throw new RuntimeError($"Y argument for Spawn must be an integer but got {Yobj}.", Token);
        
        bool sucess = Interpreter.WallEInstance.Spawn(x,y);
        if(!sucess)
           throw new RuntimeError($"Spawn command failed. Position ({Xobj},{Yobj}) might be out of canvas bounds.", Token);
        

    }
}