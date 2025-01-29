namespace Core.Models;

internal record ReadFromStdinStatement(string Variable) : IStatement
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}