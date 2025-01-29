namespace Core.Models;

internal record EqualsBooleanExpression(string Variable, int Literal) : IBooleanExpression
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}