using Core.Models;

namespace Core.CodeGen;

internal class VariablesVisitor : IVisitor
{
    private readonly HashSet<IBlock> _seen = [];
    private readonly HashSet<string> _variables = [];

    public IReadOnlySet<string> Variables => _variables;

    public void Visit(ConditionalBlock block)
    {
        if (!_seen.Add(block))
        {
            return;
        }

        block.Condition.Accept(this);
        block.True.Accept(this);
        block.False.Accept(this);
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        _variables.Add(expression.Variable);
    }

    public void Visit(LessBooleanExpression expression)
    {
        _variables.Add(expression.Variable);
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        _variables.Add(statement.Variable);
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        _variables.Add(statement.Variable);
    }

    public void Visit(ReadFromStdinStatement statement)
    {
        _variables.Add(statement.Variable);
    }

    public void Visit(SimpleBlock block)
    {
        if (!_seen.Add(block))
        {
            return;
        }

        block.Statement.Accept(this);
        block.Next?.Accept(this);
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        _variables.Add(statement.LHS);
        _variables.Add(statement.RHS);
    }

    public void Visit(StartBlock block)
    {
        block.FirstBlock.Accept(this);
    }

    public void Visit(EndBlock block) { }
}