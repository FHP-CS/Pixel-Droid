using System;
using System.Collections.Generic;
using PixelWallE.Execution;
using System.Threading.Tasks; // for delay


// (root, holding a list of statements)
// Represents the entire program: a list of statements
public class ProgramNode : AstNode
{
    public List<StatementNode> Statements { get; } = new List<StatementNode>();
    public override string ToString() => string.Join("\n", Statements);
    public override Task Execute(Interpreter interpreter) => throw new NotImplementedException("ProgramNode is run by the interpreter, not executed as a single statement.");
    public override object Evaluate(Interpreter interpreter) => throw new NotImplementedException("ProgramNode does not evaluate to a value.");
}