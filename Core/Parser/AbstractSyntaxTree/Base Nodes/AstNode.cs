using PixelWallE.Runtime.Commands;
using System.Collections.Generic;

// --- Base Nodes ---
public abstract class AstNode
{
    // We'll add an Execute(Interpreter interpreter) method here later for the interpretation phase.
    public abstract override string ToString(); // For debugging/visualization
    // public abstract object Evaluate();

}
