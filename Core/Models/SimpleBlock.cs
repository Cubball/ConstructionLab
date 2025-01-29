namespace Core.Models;

internal record SimpleBlock(IStatement Statement, IBlock? Next) : IBlock
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}