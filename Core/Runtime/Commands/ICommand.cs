// Runtime/Commands/ICommand.cs
using PixelWallE.Common;
using PixelWallE.Parser;
using PixelWallE.Runtime; // Assuming InterpreterContext will be in Runtime namespace

namespace PixelWallE.Runtime.Commands;

// Represents a single executable command in the Wall-E language
public interface ICommand
{
    // Executes the command using the provided context.
    // Returns true if execution should continue, false if it should stop (e.g., error).
    void Execute(InterpreterContext context);

    // Property to store original token for error reporting (optional but helpful)
    Token? SourceToken { get; }
}