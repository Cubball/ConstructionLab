namespace Core.Models;

internal record VariableToVariableAssignmentStatement(string LHS, int RHS) : IStatement
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}