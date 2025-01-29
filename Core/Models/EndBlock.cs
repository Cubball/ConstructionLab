namespace Core.Models;

internal class EndBlock : IBlock
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}