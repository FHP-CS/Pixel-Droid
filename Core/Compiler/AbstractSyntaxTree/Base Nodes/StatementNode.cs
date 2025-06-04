// Represents a statement (a line of code that performs an action)
using System;
using PixelWallE.Execution;

public abstract class StatementNode : AstNode
{
    public sealed override object Evaluate(Interpreter interpreter)
    {
        throw new NotImplementedException("Statement nodes are executed, not evaluated to a value.");
    }
    // Execute is now abstract here, forcing concrete statement nodes to implement it.
    public abstract override void Execute(Interpreter interpreter);
}