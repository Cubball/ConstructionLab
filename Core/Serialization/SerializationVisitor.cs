using System.Text;
using Core.Models;

namespace Core.Serialization;

internal class SerializationVisitor(
    IReadOnlyDictionary<IBlock, Guid> ids) : IVisitor
{
    private readonly IReadOnlyDictionary<IBlock, Guid> _ids = ids;
    private readonly HashSet<IBlock> _seen = [];
    private readonly StringBuilder _sb = new();

    public string String => _sb.ToString();

    public void Visit(ConditionalBlock block)
    {
        if (!_seen.Add(block))
        {
            return;
        }

        var id = _ids[block];
        var trueId = _ids[block.True];
        var falseId = _ids[block.False];
        _sb.Append(id);
        _sb.Append(',');
        _sb.Append(nameof(ConditionalBlock));
        _sb.Append(',');
        block.Condition.Accept(this);
        _sb.Append(',');
        _sb.Append(trueId);
        _sb.Append(',');
        _sb.Append(falseId);
        _sb.Append(';');

        block.True.Accept(this);
        block.False.Accept(this);
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        _sb.Append(nameof(EqualsBooleanExpression));
        _sb.Append(',');
        _sb.Append(expression.Variable);
        _sb.Append(',');
        _sb.Append(expression.Literal);
    }

    public void Visit(LessBooleanExpression expression)
    {
        _sb.Append(nameof(LessBooleanExpression));
        _sb.Append(',');
        _sb.Append(expression.Variable);
        _sb.Append(',');
        _sb.Append(expression.Literal);
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        _sb.Append(nameof(LiteralToVariableAssignmentStatement));
        _sb.Append(',');
        _sb.Append(statement.Variable);
        _sb.Append(',');
        _sb.Append(statement.Literal);
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        _sb.Append(nameof(PrintToStdoutStatement));
        _sb.Append(',');
        _sb.Append(statement.Variable);
    }

    public void Visit(ReadFromStdinStatement statement)
    {
        _sb.Append(nameof(ReadFromStdinStatement));
        _sb.Append(',');
        _sb.Append(statement.Variable);
    }

    public void Visit(SimpleBlock block)
    {
        var id = _ids[block];
        var nextId = _ids[block.Next];
        _sb.Append(id);
        _sb.Append(',');
        _sb.Append(nameof(SimpleBlock));
        _sb.Append(',');
        block.Statement.Accept(this);
        _sb.Append(',');
        _sb.Append(nextId);
        _sb.Append(';');

        block.Next?.Accept(this);
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        _sb.Append(nameof(VariableToVariableAssignmentStatement));
        _sb.Append(',');
        _sb.Append(statement.LHS);
        _sb.Append(',');
        _sb.Append(statement.RHS);
    }

    public void Visit(StartBlock block)
    {
        var id = Guid.NewGuid(); // the id of StartBlock does not really matter
        var firstBlockId = _ids[block.FirstBlock];
        _sb.Append(id);
        _sb.Append(',');
        _sb.Append(nameof(StartBlock));
        _sb.Append(',');
        _sb.Append(firstBlockId);
        _sb.Append(';');

        block.FirstBlock.Accept(this);
    }

    public void Visit(EndBlock block)
    {
        var id = _ids[block];
        _sb.Append(id);
        _sb.Append(',');
        _sb.Append(nameof(EndBlock));
        _sb.Append(';');
    }
}