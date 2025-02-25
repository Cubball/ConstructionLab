namespace Core.Models;

public sealed class EqualsBooleanExpression : IBooleanExpression
{
    public required string Variable { get; init; }

    public required int Literal { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}