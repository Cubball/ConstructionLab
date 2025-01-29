namespace Core.Models;

internal record PrintToStdoutStatement(string Variable) : IStatement
{
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}