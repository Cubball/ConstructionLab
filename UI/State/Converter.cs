using Core.Models;
using Core.Parsing;
using ConditionalBlockControl = UI.Components.ConditionalBlock;
using EndBlockControl = UI.Components.EndBlock;
using SimpleBlockControl = UI.Components.SimpleBlock;
using StartBlockControl = UI.Components.StartBlock;

namespace UI.State;

internal class Converter
{
    public static StartBlock Convert(List<Control> controls)
    {
        var startBlock = FindStartBlock(controls);
        var blocks = MapBlocks(controls);
        MapConnections(controls, startBlock, blocks);
        return startBlock;
    }

    private static StartBlock FindStartBlock(List<Control> controls)
    {
        var found = false;
        foreach (var control in controls)
        {
            if (control is not StartBlockControl startBlock)
            {
                continue;
            }

            if (found)
            {
                throw new ConversionException("Multiple start blocks found");
            }

            found = true;
        }

        if (found)
        {
            return new StartBlock();
        }

        throw new ConversionException("Start block not found");
    }

    private static Dictionary<Control, IBlock> MapBlocks(List<Control> controls)
    {
        var dictionary = new Dictionary<Control, IBlock>();
        foreach (var control in controls)
        {
            switch (control)
            {
                case StartBlockControl:
                    break;
                case EndBlockControl endBlock:
                    dictionary.Add(endBlock, new EndBlock());
                    break;
                case SimpleBlockControl simpleBlock:
                    var statement = Parser.ParseStatement(simpleBlock.Operation);
                    dictionary.Add(simpleBlock, new SimpleBlock { Statement = statement });
                    break;
                case ConditionalBlockControl conditionalBlock:
                    var condition = Parser.ParseBooleanExpression(conditionalBlock.Operation);
                    dictionary.Add(conditionalBlock, new ConditionalBlock { Condition = condition });
                    break;
                default:
                    throw new ConversionException($"Unknown block type: {control.GetType().Name}");
            }
        }

        return dictionary;
    }

    private static void MapConnections(List<Control> controls, StartBlock startBlock, Dictionary<Control, IBlock> blocks)
    {
        foreach (var control in controls)
        {
            switch (control)
            {
                case StartBlockControl startBlockControl:
                    if (startBlockControl.NextArrow.Destination is null)
                    {
                        throw new ConversionException("Start block should have a next block");
                    }

                    var startBlockNext = ArrowsManager.CurrentInstance.GetDestination(startBlockControl.NextArrow.Destination)
                        ?? throw new ConversionException("Next block not found");
                    startBlock.FirstBlock = blocks[startBlockNext];
                    break;
                case EndBlockControl endBlock:
                    if (endBlock.Destination.Origin is null)
                    {
                        throw new ConversionException("End block should have a block before it");
                    }
                    break;
                case SimpleBlockControl simpleBlock:
                    if (simpleBlock.NextArrow.Destination is null)
                    {
                        throw new ConversionException("Simple block should have a next block");
                    }

                    if (simpleBlock.Destination.Origin is null)
                    {
                        throw new ConversionException("Simple block should have a block before it");
                    }


                    var simpleBlockNext = ArrowsManager.CurrentInstance.GetDestination(simpleBlock.NextArrow.Destination)
                        ?? throw new ConversionException("Next block not found");
                    (blocks[simpleBlock] as SimpleBlock)!.Next = blocks[simpleBlockNext];
                    break;
                case ConditionalBlockControl conditionalBlock:
                    if (conditionalBlock.TrueArrow.Destination is null)
                    {
                        throw new ConversionException("Conditional block should have a true block");
                    }

                    if (conditionalBlock.FalseArrow.Destination is null)
                    {
                        throw new ConversionException("Conditional block should have a false block");
                    }

                    if (conditionalBlock.Destination.Origin is null)
                    {
                        throw new ConversionException("Conditional block should have a block before it");
                    }

                    var conditionalBlockTrue = ArrowsManager.CurrentInstance.GetDestination(conditionalBlock.TrueArrow.Destination)
                        ?? throw new ConversionException("True block not found");
                    var conditionalBlockFalse = ArrowsManager.CurrentInstance.GetDestination(conditionalBlock.FalseArrow.Destination)
                        ?? throw new ConversionException("False block not found");
                    (blocks[conditionalBlock] as ConditionalBlock)!.True = blocks[conditionalBlockTrue];
                    (blocks[conditionalBlock] as ConditionalBlock)!.False = blocks[conditionalBlockFalse];
                    break;
                default:
                    throw new ConversionException($"Unknown block type: {control.GetType().Name}");
            }
        }
    }
}