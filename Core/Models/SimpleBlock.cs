namespace Core.Models;

internal class SimpleBlock : IBlock
{
    public required IStatement Statement { get; init; }

    public IBlock? Next { get; set; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}