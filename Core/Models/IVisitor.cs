namespace Core.Models;

internal interface IVisitor
{
    void Visit(ConditionalBlock block);

    void Visit(EqualsBooleanExpression expression);

    void Visit(LessBooleanExpression expression);

    void Visit(LiteralToVariableAssignmentStatement statement);

    void Visit(PrintToStdoutStatement statement);

    void Visit(ReadFromStdinStatement statement);

    void Visit(SimpleBlock block);

    void Visit(VariableToVariableAssignmentStatement statement);
}