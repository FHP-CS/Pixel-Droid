// Represents a statement (a line of code that performs an action)
using System;
using PixelWallE.Execution;
using PixelWallE.Common;
using System.Threading.Tasks; // for delay


public abstract class StatementNode : AstNode
{
    public abstract Token Token {get;}
    public sealed override object Evaluate(Interpreter interpreter)
    {
        throw new NotImplementedException("Statement nodes are executed, not evaluated to a value.");
    }
    // Execute is now abstract here, forcing concrete statement nodes to implement it.
    public abstract override Task Execute(Interpreter interpreter);
}