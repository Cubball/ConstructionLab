using Core.Models;

namespace Core.Validation;

// TODO: public?
internal class Validator
{
    private const int MaxDiagramsCount = 100;
    private const int MaxBlocksPerDiagramCount = 100;
    private const int MaxVariablesPerDiagramCount = 100;

    public static void Validate(List<StartBlock> startBlocks)
    {
        if (startBlocks is null)
        {
            throw new ValidationException($"{nameof(startBlocks)} could not be null");
        }

        if (startBlocks.Count > MaxDiagramsCount)
        {
            throw new ValidationException($"There could be up to {MaxDiagramsCount} diagrams");
        }

        if (startBlocks.Count == 0)
        {
            throw new ValidationException("There should be at least 1 diagram");
        }

        foreach (var block in startBlocks)
        {
            if (block is null)
            {
                throw new ValidationException($"{nameof(StartBlock)} could not be null");
            }

            var visitor = new ValidatingVisitor();
            block.Accept(visitor);
            if (visitor.BlocksCount > MaxBlocksPerDiagramCount)
            {
                throw new ValidationException($"There could be up to {MaxBlocksPerDiagramCount} blocks in each diagram");
            }

            if (visitor.VariablesCount > MaxVariablesPerDiagramCount)
            {
                throw new ValidationException($"There could be up to {MaxVariablesPerDiagramCount} variables in each diagram");
            }
        }
    }
}