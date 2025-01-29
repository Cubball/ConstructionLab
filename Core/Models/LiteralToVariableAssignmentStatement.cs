namespace Core.Models;

internal record LiteralToVariableAssignmentStatement(string Variable, int Literal) : IStatement
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}