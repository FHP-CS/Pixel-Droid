// // Runtime/Commands/ColorCommand.cs
// using PixelWallE.Common;
// using PixelWallE.Runtime;

// namespace PixelWallE.Runtime.Commands;

// public class ColorCommand : ICommand
// {
//     public string ColorName { get; }
//      public Token? SourceToken { get; }

//     public ColorCommand(string colorName, Token? token = null)
//     {
//         ColorName = colorName;
//         SourceToken = token;
//     }

//     public void Execute(InterpreterContext context)
//     {
//         context.WallE.SetColor(ColorName);
//     }
// }