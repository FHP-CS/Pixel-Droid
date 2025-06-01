using PixelWallE.Common;
public class LabelNode : StatementNode
{
    public string Name {get;}
    public Token LabelToken { get; } 
    public LabelNode(Token labelToken)
    {
         Name = labelToken.Lexeme;
         LabelToken = labelToken;
    }
    public override string ToString()=> $"{Name}:";
}