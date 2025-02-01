using System.Collections.Concurrent;
using System.Globalization;
using Core.Models;

namespace Core.Interpretation;

internal class InterpretingVisitor(
    ConcurrentDictionary<string, int> variables,
    TextWriter @out,
    TextReader @in) : IVisitor
{
    private readonly ConcurrentDictionary<string, int> _variables = variables;
    private readonly TextWriter _out = @out;
    private readonly TextReader _in = @in;

    private bool _lastBooleanExpression;
    private IBlock? _next;

    public bool IsDone => _next is EndBlock;

    public void Next()
    {
        _next?.Accept(this);
    }

    public void Visit(ConditionalBlock block)
    {
        block.Condition.Accept(this);
        _next = _lastBooleanExpression ? block.True : block.False;
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        _lastBooleanExpression = GetOrSetDefaultVariableValue(expression.Variable) == expression.Literal;
    }

    public void Visit(LessBooleanExpression expression)
    {
        _lastBooleanExpression = GetOrSetDefaultVariableValue(expression.Variable) < expression.Literal;
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        _variables[statement.Variable] = statement.Literal;
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        _out.WriteLine(GetOrSetDefaultVariableValue(statement.Variable));
    }

    // TODO: error handling
    public void Visit(ReadFromStdinStatement statement)
    {
        _variables[statement.Variable] = int.Parse(_in.ReadLine()!, CultureInfo.InvariantCulture);
    }

    public void Visit(SimpleBlock block)
    {
        block.Statement.Accept(this);
        _next = block.Next;
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        _variables[statement.LHS] = GetOrSetDefaultVariableValue(statement.RHS);
    }

    public void Visit(StartBlock block)
    {
        _next = block.FirstBlock;
    }

    public void Visit(EndBlock block) { }

    private int GetOrSetDefaultVariableValue(string variable)
    {
        if (_variables.TryGetValue(variable, out var value))
        {
            return value;
        }

        _variables[variable] = 0;
        return 0;
    }
}