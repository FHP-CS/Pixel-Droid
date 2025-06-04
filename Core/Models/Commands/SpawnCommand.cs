// Runtime/Commands/SpawnCommand.cs
using PixelWallE.Common;
using PixelWallE.Execution;

namespace PixelWallE.Runtime.Commands;

public class SpawnCommand : ICommand
{
    public int X { get; }
    public int Y { get; }
    public Token? SourceToken { get; }

    public SpawnCommand(int x, int y, Token? token = null)
    {
        X = x;
        Y = y;
        SourceToken = token;
    }

    public void Execute(Interpreter Interpreter)
    {
        if (Interpreter.WallEInstance.Spawn(X, Y))
        {
            // As per PDF, runtime error stops execution
            throw new RuntimeError($"Spawn position ({X}, {Y}) is outside canvas bounds (W:{Interpreter.Canvas.Width}, H:{Interpreter.Canvas.Height}).", SourceToken);
        }
         // Interpreter.NotifyWallESpawned(); // Maybe Interpreter needs state tracking
    }
}