namespace Core.Models;

internal class StartBlock : IVisitable
{
    public required IBlock FirstBlock { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}