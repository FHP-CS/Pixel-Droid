using System;
using PixelWallE.Common;
using PixelWallE.Execution;
public class FillNode : StatementNode
{
    public FillNode(){}
    public override string ToString()=> "Fill()";
    public override void Execute(Interpreter interpreter)
    {
        throw new NotImplementedException();
    }
}
