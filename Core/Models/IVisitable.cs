namespace Core.Models;

internal interface IVisitable
{
    void Accept(IVisitor visitor);
}