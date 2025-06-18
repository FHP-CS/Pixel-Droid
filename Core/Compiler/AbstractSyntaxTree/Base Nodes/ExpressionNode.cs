// Represents an expression that evaluates to a value
using PixelWallE.Execution;
using PixelWallE.Common;
using System.Threading.Tasks; // for delay
using System;

public abstract class ExpressionNode : AstNode
{
    public sealed override Task Execute(Interpreter interpreter)
    {
        // expressions are evaluated, not executed
        throw new NotImplementedException("Expression nodes are evaluated, not executed as statements directly.");
    }
    public abstract override object Evaluate(Interpreter interpreter);
}