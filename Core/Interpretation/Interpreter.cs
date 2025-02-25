using System.Collections.Concurrent;
using Core.Models;
using Core.Validation;

namespace Core.Interpretation;

public static class Interpreter
{
    public static void Run(List<StartBlock> startBlocks, TextWriter? @out = null, TextReader? @in = null)
    {
        Validator.Validate(startBlocks);
        @out ??= Console.Out;
        @in ??= Console.In;
        var variables = new ConcurrentDictionary<string, int>();
        foreach (var block in startBlocks)
        {
            var visitor = new InterpretingVisitor(variables, @out, @in);
            block.Accept(visitor);
            new Thread(() =>
            {
                while (!visitor.IsDone)
                {
                    visitor.Next();
                }
            }).Start();
        }
    }
}