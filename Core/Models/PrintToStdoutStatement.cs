namespace Core.Models;

public sealed class PrintToStdoutStatement : IStatement
{
    public required string Variable { get; init; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}