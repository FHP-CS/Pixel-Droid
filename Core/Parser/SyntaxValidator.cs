// Parser/SyntaxValidator.cs
using PixelWallE.Common;
using PixelWallE.Runtime.Commands;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE.Parser;

public class SyntaxValidator
{
    private readonly List<ICommand> _commands;
    private readonly List<ParsingError> _errors = new List<ParsingError>();

    public SyntaxValidator(List<ICommand> commands)
    {
        _commands = commands;
    }

    public List<ParsingError> Validate()
    {
        _errors.Clear();

        if (_commands.Count == 0)
        {
            // Allow empty code? Or require Spawn? Let's assume Spawn is required if code exists.
            // If _tokens was not empty but _commands is, Parser failed.
            // If _tokens was empty, it's valid empty input.
            // This check might be redundant if Parser handles empty input.
             return _errors;
        }

        // Rule 1: Must start with Spawn
        if (_commands[0] is not SpawnCommand)
        {
            // Try to find the first token's position or default to line 1
            int line = _commands[0].SourceToken?.Line ?? 1;
            int col = _commands[0].SourceToken?.Column ?? 1;
             _errors.Add(new ParsingError("Code must start with a Spawn command.", line, col, ErrorType.Semantic));
        }

        // Rule 2: Spawn only allowed once
        int spawnCount = _commands.Count(cmd => cmd is SpawnCommand);
        if (spawnCount > 1)
        {
            // Find the second spawn command to report its position
            var secondSpawn = _commands.Skip(1).FirstOrDefault(cmd => cmd is SpawnCommand);
             int line = secondSpawn?.SourceToken?.Line ?? _commands[1].SourceToken?.Line ?? 1;
             int col = secondSpawn?.SourceToken?.Column ?? _commands[1].SourceToken?.Column ?? 1;
            _errors.Add(new ParsingError("Spawn command can only appear once at the beginning of the code.", line, col, ErrorType.Semantic));
        }

        // TODO: Validate GoTo targets when implemented
        // Build label map
        // Check each GoTo target exists in map

        // TODO: Add other semantic checks (variable usage, type checking, etc.)

        return _errors;
    }
}