namespace Core.Models;

public sealed class StartBlock : IVisitable
{
    public IBlock FirstBlock { get; set; } = default!;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}