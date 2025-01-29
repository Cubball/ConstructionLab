namespace Core.Models;

internal record LessBooleanExpression(string Variable, int Literal) : IBooleanExpression
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}