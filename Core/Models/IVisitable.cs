namespace Core.Models;

public interface IVisitable
{
    void Accept(IVisitor visitor);
}