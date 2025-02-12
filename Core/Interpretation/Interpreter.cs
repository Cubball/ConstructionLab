using System.Collections.Concurrent;
using Core.Models;
using Core.Validation;

namespace Core.Interpretation;

public static class Interpreter
{
    public static void Run(List<StartBlock> startBlocks, TextWriter? @out = null, TextReader? @in = null, CancellationToken cancellationToken = default)
    {
        Validator.Validate(startBlocks);
        @out ??= Console.Out;
        @in ??= Console.In;
        var variables = new ConcurrentDictionary<string, int>();
        var runningCount = startBlocks.Count;
        var threads = new List<Thread>();
        foreach (var block in startBlocks)
        {
            var visitor = new InterpretingVisitor(variables, @out, @in);
            block.Accept(visitor);
            var thread = new Thread(() =>
            {
                while (!visitor.IsDone && !cancellationToken.IsCancellationRequested)
                {
                    visitor.Next();
                }

                if (Interlocked.Decrement(ref runningCount) == 0)
                {
                    ExecutionFinished?.Invoke(null, EventArgs.Empty);
                }
            });
            threads.Add(thread);
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public static event EventHandler? ExecutionFinished;
}