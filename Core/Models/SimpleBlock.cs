namespace Core.Models;

public sealed class SimpleBlock : IBlock
{
    public required IStatement Statement { get; init; }

    public IBlock Next { get; set; } = default!;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}