namespace Core.Models;

public sealed class ReadFromStdinStatement : IStatement
{
    public required string Variable { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}