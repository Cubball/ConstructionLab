namespace Core.Models;

internal class ConditionalBlock : IBlock
{
    public required IBooleanExpression Condition { get; init; }

    public IBlock True { get; set; } = default!;

    public IBlock False { get; set; } = default!;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}