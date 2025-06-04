using System;
using PixelWallE.Common;
using PixelWallE.Execution;
//GoTo[Label] ()
public class GoToNode: StatementNode
{
    public string labelName {get;}
    public Token labelToken {get;}
    public ExpressionNode Condition {get;}

    public GoToNode(Token label, ExpressionNode condition)
    {
        labelName = label.Lexeme;
        labelToken = label;
        Condition = condition;
    }
    public override string ToString() => $"GoTo [{labelName}] ({Condition})";
    public override void Execute(Interpreter Interpreter)
    {
        int address = Interpreter.GetLabelAddress(labelName,labelToken);
        Interpreter.GoToAddress(address);
    }
}