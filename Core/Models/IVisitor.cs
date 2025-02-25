namespace Core.Models;

public interface IVisitor
{
    void Visit(ConditionalBlock block);

    void Visit(EqualsBooleanExpression expression);

    void Visit(LessBooleanExpression expression);

    void Visit(LiteralToVariableAssignmentStatement statement);

    void Visit(PrintToStdoutStatement statement);

    void Visit(ReadFromStdinStatement statement);

    void Visit(SimpleBlock block);

    void Visit(VariableToVariableAssignmentStatement statement);

    void Visit(StartBlock block);

    void Visit(EndBlock block);
}