using Core.Models;

namespace Core.Validation;

internal class ValidatingVisitor : IVisitor
{
    private readonly HashSet<IBlock> _blocks = [];
    private readonly HashSet<string> _variables = [];

    private bool _seenStartBlock;

    public int BlocksCount => _blocks.Count + (_seenStartBlock ? 1 : 0);

    public int VariablesCount => _variables.Count;

    public void Visit(ConditionalBlock block)
    {
        ThrowIfHaveNotSeenStartBlock(nameof(ConditionalBlock));
        if (!_blocks.Add(block))
        {
            return;
        }

        if (block.Condition is null)
        {
            throw new ValidationException($"{nameof(ConditionalBlock)} should have a non-null {nameof(ConditionalBlock.Condition)}");
        }

        if (block.True is null)
        {
            throw new ValidationException($"{nameof(ConditionalBlock)} should have a non-null {nameof(ConditionalBlock.True)} branch");
        }

        if (block.False is null)
        {
            throw new ValidationException($"{nameof(ConditionalBlock)} should have a non-null {nameof(ConditionalBlock.False)} branch");
        }

        block.Condition.Accept(this);
        block.True.Accept(this);
        block.False.Accept(this);
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        ValidateVariableName(expression.Variable, nameof(EqualsBooleanExpression.Variable));
        _variables.Add(expression.Variable);
    }

    public void Visit(LessBooleanExpression expression)
    {
        ValidateVariableName(expression.Variable, nameof(LessBooleanExpression.Variable));
        _variables.Add(expression.Variable);
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        ValidateVariableName(statement.Variable, nameof(LiteralToVariableAssignmentStatement.Variable));
        _variables.Add(statement.Variable);
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        ValidateVariableName(statement.Variable, nameof(PrintToStdoutStatement.Variable));
        _variables.Add(statement.Variable);
    }

    public void Visit(ReadFromStdinStatement statement)
    {
        ValidateVariableName(statement.Variable, nameof(PrintToStdoutStatement.Variable));
        _variables.Add(statement.Variable);
    }

    public void Visit(SimpleBlock block)
    {
        ThrowIfHaveNotSeenStartBlock(nameof(ConditionalBlock));
        if (!_blocks.Add(block))
        {
            return;
        }

        if (block.Statement is null)
        {
            throw new ValidationException($"{nameof(SimpleBlock)} should have a non-null {nameof(SimpleBlock.Statement)}");
        }

        if (block.Next is null)
        {
            throw new ValidationException($"{nameof(SimpleBlock)} should have a non-null {nameof(SimpleBlock.Next)} block");
        }

        block.Statement.Accept(this);
        block.Next.Accept(this);
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        ValidateVariableName(statement.LHS, nameof(VariableToVariableAssignmentStatement.LHS));
        _variables.Add(statement.LHS);
        ValidateVariableName(statement.RHS, nameof(VariableToVariableAssignmentStatement.RHS));
        _variables.Add(statement.RHS);
    }

    public void Visit(StartBlock block)
    {
        if (_seenStartBlock)
        {
            throw new ValidationException($"There could only be one {nameof(StartBlock)}");
        }

        if (block.FirstBlock is null)
        {
            throw new ValidationException($"{nameof(StartBlock)} should have a non-null {nameof(StartBlock.FirstBlock)}");
        }

        _seenStartBlock = true;
        block.FirstBlock.Accept(this);
    }

    public void Visit(EndBlock block)
    {
        ThrowIfHaveNotSeenStartBlock(nameof(ConditionalBlock));
        _blocks.Add(block);
    }

    private void ThrowIfHaveNotSeenStartBlock(string blockName)
    {
        if (!_seenStartBlock)
        {
            throw new ValidationException($"{blockName} cannot appear before {nameof(StartBlock)}");
        }
    }

    private static void ValidateVariableName(string variableName, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(variableName))
        {
            throw new ValidationException($"{propertyName} should not be null or empty");
        }

        if (!char.IsLetter(variableName[0]) && variableName[0] != '_')
        {
            throw new ValidationException($"{propertyName} should start with a letter or underscore");
        }

        if (variableName.Any(char.IsWhiteSpace))
        {
            throw new ValidationException($"{propertyName} cannot contain whitespace");
        }
    }
}