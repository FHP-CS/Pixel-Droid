// Runtime/Commands/SpawnCommand.cs
using PixelWallE.Common;
using PixelWallE.Runtime;

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

    public void Execute(InterpreterContext context)
    {
        if (!context.WallE.Spawn(X, Y))
        {
            // As per PDF, runtime error stops execution
            throw new RuntimeException($"Spawn position ({X}, {Y}) is outside canvas bounds (W:{context.Canvas.Width}, H:{context.Canvas.Height}).", SourceToken);
        }
         // context.NotifyWallESpawned(); // Maybe context needs state tracking
    }
}