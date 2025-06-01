using PixelWallE.Common;
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
}