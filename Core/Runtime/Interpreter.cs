// Runtime/Interpreter.cs
using PixelWallE.Common;
using PixelWallE.Models;
using PixelWallE.Runtime.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PixelWallE.Runtime;

// Holds the state needed during interpretation
public class InterpreterContext
{
    public WallE WallE { get; }
    public PixelCanvas Canvas { get; }
    // TODO: Add variable storage: public Dictionary<string, object> Variables { get; } = new();
    // TODO: Add label map: public Dictionary<string, int> LabelMap { get; } = new();
    public bool HasError { get; set; } = false;
    public ParsingError? Error { get; set; } = null;

    public InterpreterContext(WallE wallE, PixelCanvas canvas)
    {
        WallE = wallE;
        Canvas = canvas;
    }
}

// Custom Exception for Runtime Errors
public class RuntimeException : System.Exception
{
    public Token? Token { get; } // Optional: Token related to the error

    public RuntimeException(string message, Token? token = null) : base(message)
    {
        Token = token;
    }
     public RuntimeException(string message, Exception innerException, Token? token = null) : base(message, innerException)
    {
        Token = token;
    }
}


public class Interpreter
{
    public ParsingError? Run(List<ICommand> commands, WallE wallE, PixelCanvas canvas)
    {
        var context = new InterpreterContext(wallE, canvas);
        // TODO: Pre-scan commands to build label map for GoTo

        int instructionPointer = 0;

        // Basic check: Need Spawn command executed first (validator should ensure it exists at [0])
        bool isSpawned = false; // Track if Spawn has successfully run


        // --- PDF Rule: Execution starts from the current canvas state ---
        // --- DO NOT CLEAR CANVAS HERE ---
        // canvas.Clear(Avalonia.Media.Colors.White);


        while (instructionPointer < commands.Count && !context.HasError)
        {
            ICommand command = commands[instructionPointer];
            Debug.WriteLine($"Executing: {command.GetType().Name} at index {instructionPointer}");

            try
            {
                 // Runtime check: Most commands require Wall-E to be spawned
                 if (!isSpawned && !(command is SpawnCommand))
                 {
                     throw new RuntimeException("Cannot execute commands before Wall-E is successfully spawned.", command.SourceToken);
                 }

                 command.Execute(context);

                 // Mark spawned state *after* successful execution of SpawnCommand
                 if (command is SpawnCommand) {
                     isSpawned = true;
                 }

                // Increment pointer for next command (unless it's a jump)
                // TODO: Handle jumps (GoTo) - they will modify instructionPointer directly
                instructionPointer++;
            }
            catch (RuntimeException rex)
            {
                int line = rex.Token?.Line ?? command.SourceToken?.Line ?? 0;
                int col = rex.Token?.Column ?? command.SourceToken?.Column ?? 0;
                context.Error = new ParsingError($"Runtime Error: {rex.Message}", line, col, ErrorType.Runtime);
                context.HasError = true;
                // Stop execution as per PDF
            }
            catch (Exception ex) // Catch unexpected runtime exceptions
            {
                int line = command.SourceToken?.Line ?? 0;
                int col = command.SourceToken?.Column ?? 0;
                context.Error = new ParsingError($"Unexpected Runtime Error: {ex.Message}", line, col, ErrorType.Runtime);
                context.HasError = true;
                // Stop execution
            }
        }

        if (context.HasError)
        {
             Debug.WriteLine($"Execution stopped due to error: {context.Error}");
        }
        else
        {
            Debug.WriteLine("Execution finished successfully.");
        }

        return context.Error; // Return null if no error, or the error object
    }
}