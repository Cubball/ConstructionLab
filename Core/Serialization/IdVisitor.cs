using Core.Models;

namespace Core.Serialization;

internal sealed class IdVisitor : IVisitor
{
    private readonly Dictionary<IBlock, Guid> _ids = [];

    public IReadOnlyDictionary<IBlock, Guid> Ids => _ids;

    public void Visit(ConditionalBlock block)
    {
        if (AddIfFirstTimeSeeing(block))
        {
            block.True.Accept(this);
            block.False.Accept(this);
        }
    }

    public void Visit(SimpleBlock block)
    {
        if (AddIfFirstTimeSeeing(block))
        {
            block.Next.Accept(this);
        }
    }

    public void Visit(StartBlock block)
    {
        block.FirstBlock.Accept(this);
    }

    public void Visit(EndBlock block)
    {
        _ = AddIfFirstTimeSeeing(block);
    }

    public void Visit(EqualsBooleanExpression expression) { }

    public void Visit(LessBooleanExpression expression) { }

    public void Visit(LiteralToVariableAssignmentStatement statement) { }

    public void Visit(PrintToStdoutStatement statement) { }

    public void Visit(ReadFromStdinStatement statement) { }

    public void Visit(VariableToVariableAssignmentStatement statement) { }

    private bool AddIfFirstTimeSeeing(IBlock block)
    {
        return _ids.TryAdd(block, Guid.NewGuid());
    }
}