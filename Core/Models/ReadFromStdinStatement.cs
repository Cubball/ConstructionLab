namespace Core.Models;

internal class ReadFromStdinStatement : IStatement
{
    public required string Variable { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}