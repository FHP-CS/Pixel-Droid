using System.Collections.Generic;
using System;
using System.Threading.Tasks; // for delay

using PixelWallE.Execution;
using PixelWallE.Common;

// --- Base Nodes ---
public abstract class AstNode
{
    public virtual Task Execute(Interpreter interpreter)
    {
        // Default implementation for nodes that are not directly executable
        return Task.CompletedTask; // << CHANGED
    }
    // Method for expressions to compute their value
    // Returns object because expressions can evaluate to different types (int, string, bool).
    public virtual object Evaluate(Interpreter interpreter)
    {
        // Default implementation for nodes that are not expressions.
        throw new NotImplementedException($"{GetType().Name} cannot be evaluated to a value.");
    }

    public abstract override string ToString();
}
