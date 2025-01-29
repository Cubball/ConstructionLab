using Core.Models;

namespace Core.CodeGen;

internal class ReferenceCountingVisitor : IVisitor
{
    private readonly Dictionary<IBlock, int> _counts = [];

    public IReadOnlyDictionary<IBlock, int> Counts => _counts;

    public void Visit(ConditionalBlock conditionalBlock)
    {
        CountBlock(conditionalBlock);
    }

    public void Visit(SimpleBlock simpleBlock)
    {
        CountBlock(simpleBlock);
    }

    public void Visit(EqualsBooleanExpression equalsBooleanExpression) { }

    public void Visit(LessBooleanExpression lessBooleanExpression) { }

    public void Visit(LiteralToVariableAssignmentStatement literalToVariableAssignmentStatement) { }

    public void Visit(PrintToStdoutStatement printToStdoutStatement) { }

    public void Visit(ReadFromStdinStatement readFromStdinStatement) { }

    public void Visit(VariableToVariableAssignmentStatement variableToVariableAssignmentStatement) { }

    private void CountBlock(IBlock block)
    {
        if (!_counts.TryGetValue(block, out var count))
        {
            _counts[block] = 0;
        }

        _counts[block] = count + 1;
    }
}