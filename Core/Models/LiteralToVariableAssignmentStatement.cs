namespace Core.Models;

internal class LiteralToVariableAssignmentStatement : IStatement
{
    public required string Variable { get; init; }

    public required int Literal { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}