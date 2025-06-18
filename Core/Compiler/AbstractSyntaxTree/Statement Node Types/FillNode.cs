using System;
using PixelWallE.Common;
using System.Threading.Tasks; // for delay
using PixelWallE.Execution;
public class FillNode : StatementNode
{
    public override Token Token {get; }

    public FillNode(Token token){
        Token = token;
    }
    public override string ToString()=> "Fill()";
    public override async Task Execute(Interpreter interpreter)
    {
        await interpreter.WallEInstance.Fill();
    }
}
