using System.Collections.Concurrent;
using System.Globalization;
using Core.Models;

namespace Core.Interpretation;

internal class InterpretingVisitor(
    ConcurrentDictionary<string, int> variables,
    TextWriter @out,
    TextReader @in,
    AutoResetEvent? signal = null) : IVisitor
{
    private readonly ConcurrentDictionary<string, int> _variables = variables;
    private readonly TextWriter _out = @out;
    private readonly TextReader _in = @in;
    private readonly AutoResetEvent? _signal = signal;

    private bool _lastBooleanExpression;

    public void Visit(ConditionalBlock block)
    {
        block.Condition.Accept(this);
        if (_lastBooleanExpression)
        {
            block.True.Accept(this);
        }
        else
        {
            block.False.Accept(this);
        }
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        _signal?.WaitOne();
        _lastBooleanExpression = GetOrSetDefaultVariableValue(expression.Variable) == expression.Literal;
    }

    public void Visit(LessBooleanExpression expression)
    {
        _signal?.WaitOne();
        _lastBooleanExpression = GetOrSetDefaultVariableValue(expression.Variable) < expression.Literal;
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        _signal?.WaitOne();
        _variables[statement.Variable] = statement.Literal;
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        _signal?.WaitOne();
        _out.WriteLine(GetOrSetDefaultVariableValue(statement.Variable));
    }

    // TODO: error handling
    public void Visit(ReadFromStdinStatement statement)
    {
        _signal?.WaitOne();
        _variables[statement.Variable] = int.Parse(_in.ReadLine()!, CultureInfo.InvariantCulture);
    }

    public void Visit(SimpleBlock block)
    {
        block.Statement.Accept(this);
        block.Next.Accept(this);
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        _signal?.WaitOne();
        _variables[statement.LHS] = GetOrSetDefaultVariableValue(statement.RHS);
    }

    public void Visit(StartBlock block)
    {
        block.FirstBlock.Accept(this);
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