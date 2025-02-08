using Core.Models;

namespace Core.Parsing;

public static class Parser
{
    private const string InputStart = "INPUT ";
    private const string PrintStart = "PRINT ";

    public static IBooleanExpression ParseBooleanExpression(string expression)
    {
        var parts = expression.Split('<');
        if (parts.Length == 2)
        {
            return ParseLessBooleanExpresion(parts);
        }

        parts = expression.Split("==");
        if (parts.Length == 2)
        {
            return ParseEqualBooleanExpresion(parts);
        }

        throw new ArgumentException("Invalid boolean expression");
    }

    public static IStatement ParseStatement(string statement)
    {
        statement = statement.Trim();
        if (statement.StartsWith(PrintStart, StringComparison.OrdinalIgnoreCase))
        {
            return ParsePrintStatement(statement);
        }

        if (statement.StartsWith(InputStart, StringComparison.OrdinalIgnoreCase))
        {
            return ParseInputStatement(statement);
        }

        return ParseAssignmentStatement(statement);
    }

    private static EqualsBooleanExpression ParseEqualBooleanExpresion(string[] parts)
    {
        var left = parts[0].Trim();
        var right = parts[1].Trim();
        ValidateVariableName(left);
        if (!int.TryParse(right, out var literal))
        {
            throw new ParsingException("Literal should be an integer");
        }

        return new EqualsBooleanExpression
        {
            Variable = left,
            Literal = literal
        };
    }

    private static LessBooleanExpression ParseLessBooleanExpresion(string[] parts)
    {
        var left = parts[0].Trim();
        var right = parts[1].Trim();
        ValidateVariableName(left);
        if (!int.TryParse(right, out var literal))
        {
            throw new ParsingException("Literal should be an integer");
        }

        return new LessBooleanExpression
        {
            Variable = left,
            Literal = literal
        };
    }

    private static PrintToStdoutStatement ParsePrintStatement(string statement)
    {
        var variable = statement.Replace(PrintStart, string.Empty).Trim();
        ValidateVariableName(variable);
        return new PrintToStdoutStatement
        {
            Variable = variable
        };
    }

    private static ReadFromStdinStatement ParseInputStatement(string statement)
    {
        var variable = statement.Replace(InputStart, string.Empty).Trim();
        ValidateVariableName(variable);
        return new ReadFromStdinStatement
        {
            Variable = variable
        };
    }

    private static IStatement ParseAssignmentStatement(string statement)
    {
        var parts = statement.Split('=');
        if (parts.Length != 2)
        {
            throw new ParsingException("Invalid assignment statement");
        }

        var variable = parts[0].Trim();
        ValidateVariableName(variable);
        var rhs = parts[1].Trim();
        if (char.IsDigit(rhs[0]))
        {
            if (!int.TryParse(rhs, out var literal))
            {
                throw new ParsingException("Literal should be an integer");
            }

            return new LiteralToVariableAssignmentStatement
            {
                Variable = variable,
                Literal = literal
            };
        }

        ValidateVariableName(rhs);
        return new VariableToVariableAssignmentStatement
        {
            LHS = variable,
            RHS = rhs
        };
    }

    private static void ValidateVariableName(string variableName)
    {
        if (string.IsNullOrWhiteSpace(variableName))
        {
            throw new ParsingException("Variable name could not be empty");
        }

        if (!char.IsLetter(variableName[0]) && variableName[0] != '_')
        {
            throw new ParsingException("Variable name should start with a letter or an underscore");
        }

        if (variableName.Any(char.IsWhiteSpace))
        {
            throw new ParsingException("Variable name should not contain whitespaces");
        }
    }
}