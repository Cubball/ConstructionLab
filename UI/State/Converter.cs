using Core.Models;
using Core.Parsing;
using UI.Models;
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

    public static List<Control> Convert(StartBlock startBlock)
    {
        var queue = new Queue<(ArrowOrigin, IBlock, Point)>();
        var startBlockControl = new StartBlockControl(new(100, 150));
        var seen = new Dictionary<IBlock, Control>();
        queue.Enqueue((startBlockControl.NextArrow, startBlock.FirstBlock, new(100, 650)));
        var result = new List<Control> { startBlockControl };
        while (queue.Count > 0)
        {
            var (origin, current, location) = queue.Dequeue();
            ArrowsManager.CurrentInstance.AddOrigin(origin);
            if (seen.TryGetValue(current, out var control))
            {
                origin.Destination = control switch
                {
                    SimpleBlockControl simpleBlock => simpleBlock.Destination,
                    ConditionalBlockControl conditionalBlock => conditionalBlock.Destination,
                    EndBlockControl endBlock => endBlock.Destination,
                    _ => throw new ConversionException($"Unknown block type: {control.GetType().Name}"),
                };
                continue;
            }

            var x = location.X;
            var y = location.Y;
            switch (current)
            {
                case EndBlock:
                    var endBlockControl = new EndBlockControl(new(x, y));
                    ArrowsManager.CurrentInstance.AddDestination(endBlockControl.Destination, endBlockControl);
                    origin.Destination = endBlockControl.Destination;
                    result.Add(endBlockControl);
                    seen.Add(current, endBlockControl);
                    break;
                case SimpleBlock simpleBlock:
                    var simpleBlockControl = new SimpleBlockControl(new(x, y));
                    ArrowsManager.CurrentInstance.AddDestination(simpleBlockControl.Destination, simpleBlockControl);
                    origin.Destination = simpleBlockControl.Destination;
                    simpleBlockControl.SetOperation(ToString(simpleBlock.Statement));
                    queue.Enqueue((simpleBlockControl.NextArrow, simpleBlock.Next, new(x, y + 500)));
                    result.Add(simpleBlockControl);
                    seen.Add(current, simpleBlockControl);
                    break;
                case ConditionalBlock conditionalBlock:
                    var conditionalBlockControl = new ConditionalBlockControl(new(x, y));
                    ArrowsManager.CurrentInstance.AddDestination(conditionalBlockControl.Destination, conditionalBlockControl);
                    origin.Destination = conditionalBlockControl.Destination;
                    conditionalBlockControl.SetOperation(ToString(conditionalBlock.Condition));
                    queue.Enqueue((conditionalBlockControl.TrueArrow, conditionalBlock.True, new(x, y + 500)));
                    queue.Enqueue((conditionalBlockControl.FalseArrow, conditionalBlock.False, new(x + 500, y)));
                    result.Add(conditionalBlockControl);
                    seen.Add(current, conditionalBlockControl);
                    break;
                default:
                    throw new ConversionException($"Unknown block type: {current.GetType().Name}");
            }
        }

        return result;
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

    private static string ToString(IStatement statement)
    {
        return statement switch
        {
            VariableToVariableAssignmentStatement varToVar => $"{varToVar.LHS} = {varToVar.RHS}",
            LiteralToVariableAssignmentStatement litToVar => $"{litToVar.Variable} = {litToVar.Literal}",
            PrintToStdoutStatement print => $"PRINT {print.Variable}",
            ReadFromStdinStatement read => $"INPUT {read.Variable}",
            _ => throw new ConversionException($"Unknown statement type: {statement.GetType().Name}")
        };
    }

    private static string ToString(IBooleanExpression expression)
    {
        return expression switch
        {
            LessBooleanExpression less => $"{less.Variable} < {less.Literal}",
            EqualsBooleanExpression equals => $"{equals.Variable} == {equals.Literal}",
            _ => throw new ConversionException($"Unknown expression type: {expression.GetType().Name}")
        };
    }
}