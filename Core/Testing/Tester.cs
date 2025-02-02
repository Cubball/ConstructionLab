using System.Collections.Concurrent;
using Core.Interpretation;
using Core.Models;
using Core.Validation;

namespace Core.Testing;

// TODO: separate file?
public record struct ExecutionResult(List<int> ExecutedThreads, List<string> Output);

internal record struct ExecutionState(List<int> ExecutedThreads, List<int> WorkingThreads);

internal record struct ThreadState(InterpretingVisitor Visitor, MockStdout Output);

public static class Tester
{
    public static Task<List<ExecutionResult>> Test(
        List<StartBlock> startBlocks,
        List<string> stdin,
        CancellationToken cancellationToken = default)
    {
        Validator.Validate(startBlocks);
        return Task.Run(() => TestInternal(startBlocks, stdin, cancellationToken));
    }

    private static List<ExecutionResult> TestInternal(
        List<StartBlock> startBlocks,
        List<string> stdin,
        CancellationToken cancellationToken)
    {
        var result = new List<ExecutionResult>();
        var queue = new Queue<ExecutionState>();
        queue.Enqueue(new([], Enumerable.Range(0, startBlocks.Count).ToList()));
        while (queue.Count > 0)
        {
            var (executedThreads, workingThreads) = queue.Dequeue();
            foreach (var thread in workingThreads)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return result;
                }

                var visitors = ReplayOperations(startBlocks, executedThreads, stdin);
                var (visitor, stdout) = visitors[thread];
                visitor.Next();
                var nextExecutedThreads = executedThreads.Append(thread).ToList();
                var nextWorkingThreads = visitor.IsDone
                    ? workingThreads.Where(t => t != thread).ToList()
                    : [.. workingThreads];
                result.Add(new(nextExecutedThreads, stdout.Lines));
                if (nextWorkingThreads.Count > 0)
                {
                    queue.Enqueue(new(nextExecutedThreads, nextWorkingThreads));
                }
            }
        }

        return result;
    }

    private static List<ThreadState> ReplayOperations(
        List<StartBlock> startBlocks,
        List<int> executedThreads,
        List<string> stdin)
    {
        var variables = new ConcurrentDictionary<string, int>();
        var result = new List<ThreadState>(startBlocks.Count);
        foreach (var block in startBlocks)
        {
            var stdout = new MockStdout();
            var visitor = new InterpretingVisitor(variables, stdout, new MockStdin(stdin));
            result.Add(new(visitor, stdout));
            block.Accept(visitor);
        }

        foreach (var thread in executedThreads)
        {
            var (visitor, _) = result[thread];
            visitor.Next();
        }

        return result;
    }
}