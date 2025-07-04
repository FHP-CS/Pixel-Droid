using System;
using PixelWallE.Common;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay
//GoTo[Label] ()
public class GoToNode : StatementNode
{
    public string labelName { get; }
    public override Token Token { get; }
    public Token labelToken { get; }
    public ExpressionNode Condition { get; }

    public GoToNode(Token label, ExpressionNode condition, Token MainToken)
    {
        labelName = label.Lexeme;
        labelToken = label;
        Condition = condition;
        Token = MainToken;
    }
    public override string ToString() => $"GoTo [{labelName}] ({Condition})";
    public override Task Execute(Interpreter Interpreter)
    {
        object cond = Condition.Evaluate(Interpreter);
        int address = Interpreter.GetLabelAddress(labelName, labelToken);

        if (cond is bool Jump)
        {
            if (Jump) Interpreter.GoToAddress(address);
        }
        else if (cond is int bin)
        {
            if (bin != 0) Interpreter.GoToAddress(address);
        }
        else
            throw new RuntimeError("Condition of the GoTo must be a boolean expression", Token);
        return Task.CompletedTask;
    }
}