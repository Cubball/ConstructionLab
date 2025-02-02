namespace Core.Models;

public sealed class EndBlock : IBlock
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}