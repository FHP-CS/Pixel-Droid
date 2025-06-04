//(for var <- Expression)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;

// var <- expression
public class AssignmentNode : StatementNode
{
    public Token VariableNameToken { get; }
    public ExpressionNode ValueExpression { get; }

    public AssignmentNode(Token variableNameToken, ExpressionNode valueExpression)
    {
        VariableNameToken = variableNameToken;
        ValueExpression = valueExpression;
    }
    public override string ToString() => $"{VariableNameToken.Lexeme} <- {ValueExpression}";
    public override void Execute(Interpreter Interpreter)
    {
       object value = ValueExpression.Evaluate(Interpreter);
       Interpreter.SetVariable(VariableNameToken.Lexeme, value);
    }

}