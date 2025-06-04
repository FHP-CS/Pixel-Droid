// For variables when used in an expression (e.g., the 'n' in k <- n * 2)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class VariableNode : ExpressionNode
{
    public string Name { get; }
    public Token VarToken { get; }

    public VariableNode(Token varToken)
    {
        Name = varToken.Lexeme;
        VarToken = varToken;
    }
    public override string ToString() => Name;
    public override object Evaluate(Interpreter interpreter)
    {
        return interpreter.GetVariable(Name, VarToken);// return the value of the variable saved in the dictionary before, pass the token for error context
    }
}