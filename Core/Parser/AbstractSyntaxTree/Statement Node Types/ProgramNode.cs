using System;
using System.Collections.Generic;
// (root, holding a list of statements)
// Represents the entire program: a list of statements
public class ProgramNode : AstNode
{
    public List<StatementNode> Statements { get; } = new List<StatementNode>();
    public override string ToString() => string.Join("\n", Statements);
}