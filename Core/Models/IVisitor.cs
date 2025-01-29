namespace Core.Models;

internal interface IVisitor
{
    void Visit(ConditionalBlock conditionalBlock);

    void Visit(EqualsBooleanExpression equalsBooleanExpression);

    void Visit(LessBooleanExpression lessBooleanExpression);

    void Visit(LiteralToVariableAssignmentStatement literalToVariableAssignmentStatement);

    void Visit(PrintToStdoutStatement printToStdoutStatement);

    void Visit(ReadFromStdinStatement readFromStdinStatement);

    void Visit(SimpleBlock simpleBlock);

    void Visit(VariableToVariableAssignmentStatement variableToVariableAssignmentStatement);
}