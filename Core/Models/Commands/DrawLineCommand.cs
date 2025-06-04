// // Runtime/Commands/DrawLineCommand.cs
// using PixelWallE.Common;
// using PixelWallE.Runtime;

// namespace PixelWallE.Runtime.Commands;

// public class DrawLineCommand : ICommand
// {
//     public int DirX { get; }
//     public int DirY { get; }
//     public int Distance { get; }
//      public Token? SourceToken { get; }

//     public DrawLineCommand(int dirX, int dirY, int distance, Token? token = null)
//     {
//         DirX = dirX;
//         DirY = dirY;
//         Distance = distance;
//         SourceToken = token;
//     }

//     public void Execute(InterpreterContext context)
//     {
//         if (!context.WallE.DrawLine(DirX, DirY, Distance))
//         {
//              // DrawLine currently returns bool but doesn't indicate critical error
//              // If it were to throw an exception for bad dir, we'd catch it here.
//              // For now, assume it handles invalid directions gracefully internally.
//         }
//     }
// }