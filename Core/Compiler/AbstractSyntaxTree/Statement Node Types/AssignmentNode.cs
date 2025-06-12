//(for var <- Expression)
using System;
using PixelWallE.Common;
using PixelWallE.Execution;

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
    public override void Execute(Interpreter Interpreter)
    {
       object value = ValueExpression.Evaluate(Interpreter);
       Interpreter.SetVariable(Token.Lexeme, value);
    }

}