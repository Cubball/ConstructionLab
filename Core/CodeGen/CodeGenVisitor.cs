using System.Globalization;
using System.Text;
using Core.Models;

namespace Core.CodeGen;

internal class CodeGenVisitor(IReadOnlyDictionary<IBlock, string> labels) : IVisitor
{
    private const string ContextDicitonaryVariableName = "ctx";

    private readonly IReadOnlyDictionary<IBlock, string> _labels = labels;
    private readonly HashSet<IBlock> _seen = [];
    private readonly StringBuilder _sb = new();

    public void Visit(ConditionalBlock block)
    {
        if (AppendGoToIfAleadySeen(block))
        {
            return;
        }

        AppendLabelIfExists(block);
        _sb.Append("if (");
        block.Condition.Accept(this);
        _sb.Append(")\n{\n");
        block.True.Accept(this);
        _sb.Append("}\nelse\n{\n");
        block.False.Accept(this);
        _sb.Append("}\n");
    }

    public void Visit(EqualsBooleanExpression expression)
    {
        _sb.Append(CultureInfo.InvariantCulture, $"{ContextDicitonaryVariableName}[\"{expression.Variable}\"] == {expression.Literal}");
    }

    public void Visit(LessBooleanExpression expression)
    {
        _sb.Append(CultureInfo.InvariantCulture, $"{ContextDicitonaryVariableName}[\"{expression.Variable}\"] < {expression.Literal}");
    }

    public void Visit(LiteralToVariableAssignmentStatement statement)
    {
        _sb.Append(CultureInfo.InvariantCulture, $"{ContextDicitonaryVariableName}[\"{statement.Variable}\"] = {statement.Literal};\n");
    }

    public void Visit(PrintToStdoutStatement statement)
    {
        _sb.Append(CultureInfo.InvariantCulture, $"System.Console.WriteLine({ContextDicitonaryVariableName}[\"{statement.Variable}\"]);\n");
    }

    public void Visit(ReadFromStdinStatement statement)
    {
        // TODO: error handling
        _sb.Append(CultureInfo.InvariantCulture, $"{ContextDicitonaryVariableName}[\"{statement.Variable}\"] = int.Parse(System.Console.ReadLine());\n");
    }

    public void Visit(SimpleBlock block)
    {
        if (AppendGoToIfAleadySeen(block))
        {
            return;
        }

        AppendLabelIfExists(block);
        block.Statement.Accept(this);
        block.Next?.Accept(this);
    }

    public void Visit(VariableToVariableAssignmentStatement statement)
    {
        _sb.Append(CultureInfo.InvariantCulture, $"{ContextDicitonaryVariableName}[\"{statement.LHS}\"] = {ContextDicitonaryVariableName}{statement.RHS};\n");
    }

    private bool AppendGoToIfAleadySeen(IBlock block)
    {
        if (!_seen.Add(block))
        {
            _sb.Append(CultureInfo.InvariantCulture, $"goto {_labels[block]};\n");
            return true;
        }

        return false;
    }

    private void AppendLabelIfExists(IBlock block)
    {
        if (_labels.TryGetValue(block, out var label))
        {
            _sb.Append(CultureInfo.InvariantCulture, $"{label}:\n");
        }
    }
}