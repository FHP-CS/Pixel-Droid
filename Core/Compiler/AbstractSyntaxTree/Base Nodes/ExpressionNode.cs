// Represents an expression that evaluates to a value
using PixelWallE.Execution;
using System;

public abstract class ExpressionNode : AstNode
{
    public sealed override void Execute(Interpreter interpreter)
    {
        // expressions are evaluated, not executed
        throw new NotImplementedException("Expression nodes are evaluated, not executed as statements directly.");
    }
    public abstract override object Evaluate(Interpreter interpreter);
}