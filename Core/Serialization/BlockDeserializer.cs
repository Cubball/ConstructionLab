using Core.Models;

namespace Core.Serialization;

internal class BlockDeserializer
{
    private const char BlocksSeparator = ';';
    private const char BlockPartsSeparator = ',';

    private readonly Dictionary<Guid, IBlock> _blocks = [];
    private readonly Dictionary<IBlock, Guid> _nextIds = [];
    private readonly Dictionary<IBlock, (Guid True, Guid False)> _branchIds = [];
    private readonly HashSet<IBlock> _seen = [];

    public StartBlock Deserialize(string serialized)
    {
        var blocks = serialized
            .Split(BlocksSeparator)
            .Where(static b => !string.IsNullOrWhiteSpace(b))
            .ToArray();
        var firstBlockId = DeserializeStartBlock(blocks[0]);
        foreach (var block in blocks.AsSpan(1))
        {
            DeserializeBlock(block);
        }

        return AssembleBlocks(firstBlockId);
    }

    private Guid DeserializeStartBlock(string block)
    {
        var parts = block.Split(BlockPartsSeparator);
        if (parts.Length != 3)
        {
            throw new SerializationException($"Serialized {nameof(StartBlock)} should have 2 parts");
        }

        if (parts[1] != nameof(StartBlock))
        {
            throw new SerializationException($"Expected {nameof(StartBlock)}, got {parts[1]}");
        }

        return ParseGuidOrThrow(parts[2]);
    }

    private void DeserializeBlock(string block)
    {
        var parts = block.Split(BlockPartsSeparator);
        if (parts.Length < 2)
        {
            throw new SerializationException("Each serialized block should have at least 2 parts");
        }

        var type = parts[1];
        switch (type)
        {
            case nameof(SimpleBlock):
                DeserializeSimpleBlock(parts);
                break;
            case nameof(ConditionalBlock):
                DeserializeConditionalBlock(parts);
                break;
            case nameof(EndBlock):
                DeserializeEndBlock(parts);
                break;
            default:
                throw new SerializationException($"Unrecognized block type: \"{type}\"");
        }
    }

    private StartBlock AssembleBlocks(Guid firstBlockId)
    {
        var firstBlock = _blocks.GetValueOrDefault(firstBlockId)
            ?? throw new SerializationException($"{nameof(StartBlock)} references a block with ID \"{firstBlockId}\", which does not exist");
        var startBlock = new StartBlock { FirstBlock = firstBlock };
        var queue = new Queue<IBlock>();
        queue.Enqueue(firstBlock);
        while (queue.Count > 0)
        {
            var block = queue.Dequeue();
            if (!_seen.Add(block))
            {
                continue;
            }

            switch (block)
            {
                case StartBlock:
                    throw new SerializationException($"A {nameof(StartBlock)} cannot have any other blocks going into it");
                case SimpleBlock simpleBlock:
                    if (!_nextIds.TryGetValue(simpleBlock, out var nextId))
                    {
                        throw new SerializationException($"{nameof(SimpleBlock)} does not have an ID of the next block");
                    }

                    simpleBlock.Next = GetBlockByIdOrThrow(nextId);
                    queue.Enqueue(simpleBlock.Next);
                    break;
                case ConditionalBlock conditionalBlock:
                    if (!_branchIds.TryGetValue(conditionalBlock, out var branchIds))
                    {
                        throw new SerializationException($"{nameof(ConditionalBlock)} does not have IDs of the \"true\" and \"false\" branches");
                    }

                    conditionalBlock.True = GetBlockByIdOrThrow(branchIds.True);
                    conditionalBlock.False = GetBlockByIdOrThrow(branchIds.False);
                    queue.Enqueue(conditionalBlock.True);
                    queue.Enqueue(conditionalBlock.False);
                    break;
                default:
                    break;
            }
        }

        return startBlock;
    }

    private void DeserializeSimpleBlock(string[] parts)
    {
        if (parts.Length < 3)
        {
            throw new SerializationException($"Serialized {nameof(SimpleBlock)} should have at least 3 parts");
        }

        var statement = DeserializeStatement(parts.AsSpan(2..^1));
        var block = new SimpleBlock
        {
            Statement = statement,
        };
        var id = ParseGuidOrThrow(parts[0]);
        var nextId = ParseGuidOrThrow(parts[^1]);
        _blocks[id] = block;
        _nextIds[block] = nextId;
    }

    private void DeserializeConditionalBlock(string[] parts)
    {
        if (parts.Length < 4)
        {
            throw new SerializationException($"Serialized {nameof(ConditionalBlock)} should have at least 4 parts");
        }

        var expression = DeserializeBooleanExpression(parts.AsSpan(2..^2));
        var block = new ConditionalBlock
        {
            Condition = expression,
        };
        var id = ParseGuidOrThrow(parts[0]);
        var trueId = ParseGuidOrThrow(parts[^2]);
        var falseId = ParseGuidOrThrow(parts[^1]);
        _blocks[id] = block;
        _branchIds[block] = (trueId, falseId);
    }

    private void DeserializeEndBlock(string[] parts)
    {
        var block = new EndBlock();
        var id = ParseGuidOrThrow(parts[0]);
        _blocks[id] = block;
    }

    private IStatement DeserializeStatement(Span<string> parts)
    {
        if (parts.Length < 1)
        {
            throw new SerializationException("Each serialized statement should have at least 1 part");
        }

        var type = parts[0];
        return type switch
        {
            nameof(LiteralToVariableAssignmentStatement) => ParseLiteralToVariableAssignmentStatement(parts),
            nameof(PrintToStdoutStatement) => ParsePrintToStdoutStatement(parts),
            nameof(ReadFromStdinStatement) => ParseReadFromStdinStatement(parts),
            nameof(VariableToVariableAssignmentStatement) => ParseVariableToVariableAssignmentStatement(parts),
            _ => throw new SerializationException($"Unrecognized statement type: \"{type}\""),
        };
    }

    private IBooleanExpression DeserializeBooleanExpression(Span<string> parts)
    {
        if (parts.Length < 1)
        {
            throw new SerializationException("Each serialized boolean expressiohould have at least 1 part");
        }

        var type = parts[0];
        return type switch
        {
            nameof(EqualsBooleanExpression) => ParseEqualsBooleanExpression(parts),
            nameof(LessBooleanExpression) => ParseLessBooleanExpression(parts),
            _ => throw new SerializationException($"Unrecognized boolean expression type: \"{type}\""),
        };
    }

    private LiteralToVariableAssignmentStatement ParseLiteralToVariableAssignmentStatement(Span<string> parts)
    {
        if (parts.Length != 3)
        {
            throw new SerializationException($"Serialized {nameof(LiteralToVariableAssignmentStatement)} should have 3 parts");
        }

        return new LiteralToVariableAssignmentStatement
        {
            Variable = parts[1],
            Literal = ParseIntOrThrow(parts[2]),
        };
    }

    private PrintToStdoutStatement ParsePrintToStdoutStatement(Span<string> parts)
    {
        if (parts.Length != 2)
        {
            throw new SerializationException($"Serialized {nameof(PrintToStdoutStatement)} should have 2 parts");
        }

        return new PrintToStdoutStatement
        {
            Variable = parts[1],
        };
    }

    private ReadFromStdinStatement ParseReadFromStdinStatement(Span<string> parts)
    {
        if (parts.Length != 2)
        {
            throw new SerializationException($"Serialized {nameof(ReadFromStdinStatement)} should have 2 parts");
        }

        return new ReadFromStdinStatement
        {
            Variable = parts[1],
        };
    }

    private VariableToVariableAssignmentStatement ParseVariableToVariableAssignmentStatement(Span<string> parts)
    {
        if (parts.Length != 3)
        {
            throw new SerializationException($"Serialized {nameof(VariableToVariableAssignmentStatement)} should have 3 parts");
        }

        return new VariableToVariableAssignmentStatement
        {
            LHS = parts[1],
            RHS = parts[2],
        };
    }

    private EqualsBooleanExpression ParseEqualsBooleanExpression(Span<string> parts)
    {
        if (parts.Length != 3)
        {
            throw new SerializationException($"Serialized {nameof(EqualsBooleanExpression)} should have 3 parts");
        }

        return new EqualsBooleanExpression
        {
            Variable = parts[1],
            Literal = ParseIntOrThrow(parts[2]),
        };
    }

    private LessBooleanExpression ParseLessBooleanExpression(Span<string> parts)
    {
        if (parts.Length != 3)
        {
            throw new SerializationException($"Serialized {nameof(LessBooleanExpression)} should have 3 parts");
        }

        return new LessBooleanExpression
        {
            Variable = parts[1],
            Literal = ParseIntOrThrow(parts[2]),
        };
    }

    private IBlock GetBlockByIdOrThrow(Guid id)
    {
        return _blocks.GetValueOrDefault(id)
            ?? throw new SerializationException($"A block references another block with ID \"{id}\", which does not exist");
    }

    private static Guid ParseGuidOrThrow(string @string)
    {
        return Guid.TryParse(@string, out var guid)
            ? guid
            : throw new SerializationException($"Failed to parse \"{@string}\", as GUID");
    }

    private static int ParseIntOrThrow(string @string)
    {
        if (!int.TryParse(@string, out var @int))
        {
            throw new SerializationException($"Failed to parse \"{@string}\", as an integer");
        }

        if (@int < 0)
        {
            throw new SerializationException($"Integer literal should be in range [0; {int.MaxValue}]");
        }

        return @int;
    }
}