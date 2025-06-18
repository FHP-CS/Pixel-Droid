using System;
using PixelWallE.Common;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay

public class LabelNode : StatementNode
{
    public string Name {get;}
    public override Token Token {get; }
    public LabelNode(Token labelToken)
    {
         Name = labelToken.Lexeme;
         Token = labelToken;
    }
    public override string ToString()=> $"{Name}:";
    public override Task Execute(Interpreter Interpreter){
        return Task.CompletedTask;
    }
}