namespace Core.Models;

internal record ConditionalBlock(IBooleanExpression Condition, IBlock True, IBlock False) : IBlock
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}