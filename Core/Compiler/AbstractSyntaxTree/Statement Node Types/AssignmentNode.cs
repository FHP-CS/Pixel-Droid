//(for var <- Expression)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay

// var <- expression
public class AssignmentNode : StatementNode
{
    public override Token Token { get;}
    public ExpressionNode ValueExpression { get; }

    public AssignmentNode(Token token, ExpressionNode valueExpression)
    {
        Token = token;
        ValueExpression = valueExpression;
    }
    public override string ToString() => $"{Token.Lexeme} <- {ValueExpression}";
    public override Task Execute(Interpreter Interpreter)
    {
       object value = ValueExpression.Evaluate(Interpreter);
       Interpreter.SetVariable(Token.Lexeme, value);
       return Task.CompletedTask; // Indicate completion
    }

}