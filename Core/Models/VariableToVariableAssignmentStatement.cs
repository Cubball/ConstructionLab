namespace Core.Models;

public sealed class VariableToVariableAssignmentStatement : IStatement
{
    public required string LHS { get; init; }

    public required string RHS { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}