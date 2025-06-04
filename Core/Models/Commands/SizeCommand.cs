// // Runtime/Commands/SizeCommand.cs
// using PixelWallE.Common;
// using PixelWallE.Runtime;

// namespace PixelWallE.Runtime.Commands;

// public class SizeCommand : ICommand
// {
//     public int Size { get; }
//      public Token? SourceToken { get; }

//     public SizeCommand(int size, Token? token = null)
//     {
//         Size = size;
//         SourceToken = token;
//     }

//     public void Execute(InterpreterContext context)
//     {
//         context.WallE.SetSize(Size);
//     }
// }