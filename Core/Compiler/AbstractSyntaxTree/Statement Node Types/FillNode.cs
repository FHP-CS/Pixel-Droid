using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class FillNode : StatementNode
{
    public override Token Token {get; }

    public FillNode(Token token){
        Token = token;
    }
    public override string ToString()=> "Fill()";
    public override void Execute(Interpreter interpreter)
    {
        interpreter.WallEInstance.Fill();
    }
}
