using Core.Models;

namespace Core.CodeGeneration;

internal sealed class LabelVisitor : IVisitor
{
    private readonly HashSet<IBlock> _seen = [];
    private readonly Dictionary<IBlock, string> _labels = [];

    public IReadOnlyDictionary<IBlock, string> Labels => _labels;

    public void Visit(ConditionalBlock block)
    {
        if (FirstTimeSeeing(block))
        {
            block.True.Accept(this);
            block.False.Accept(this);
        }
    }

    public void Visit(SimpleBlock block)
    {
        if (FirstTimeSeeing(block))
        {
            block.Next.Accept(this);
        }
    }

    public void Visit(EqualsBooleanExpression equalsBooleanExpression) { }

    public void Visit(LessBooleanExpression lessBooleanExpression) { }

    public void Visit(LiteralToVariableAssignmentStatement literalToVariableAssignmentStatement) { }

    public void Visit(PrintToStdoutStatement printToStdoutStatement) { }

    public void Visit(ReadFromStdinStatement readFromStdinStatement) { }

    public void Visit(VariableToVariableAssignmentStatement variableToVariableAssignmentStatement) { }

    public void Visit(StartBlock block)
    {
        block.FirstBlock.Accept(this);
    }

    public void Visit(EndBlock block) { }

    private bool FirstTimeSeeing(IBlock block)
    {
        if (_seen.Add(block))
        {
            return true;
        }

        if (!_labels.ContainsKey(block))
        {
            var index = _labels.Count + 1;
            _labels[block] = $"LABEL_{index}";
        }

        return false;
    }
}