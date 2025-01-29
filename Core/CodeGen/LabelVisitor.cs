using Core.Models;

namespace Core.CodeGen;

internal class LabelVisitor : IVisitor
{
    private readonly HashSet<IBlock> _seen = [];
    private readonly Dictionary<IBlock, string> _labels = [];

    public IReadOnlyDictionary<IBlock, string> Labels => _labels;

    public void Visit(ConditionalBlock block)
    {
        CountBlock(block);
    }

    public void Visit(SimpleBlock block)
    {
        CountBlock(block);
    }

    public void Visit(EqualsBooleanExpression equalsBooleanExpression) { }

    public void Visit(LessBooleanExpression lessBooleanExpression) { }

    public void Visit(LiteralToVariableAssignmentStatement literalToVariableAssignmentStatement) { }

    public void Visit(PrintToStdoutStatement printToStdoutStatement) { }

    public void Visit(ReadFromStdinStatement readFromStdinStatement) { }

    public void Visit(VariableToVariableAssignmentStatement variableToVariableAssignmentStatement) { }

    private void CountBlock(IBlock block)
    {
        if (_seen.Add(block))
        {
            return;
        }

        var index = _labels.Count + 1;
        _labels[block] = $"LABEL_{index}";
    }
}