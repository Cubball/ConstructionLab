using System.Collections.Concurrent;
using Core.Interpretation;
using Core.Models;

namespace Core.Testing;

internal class Tester
{
    // TODO: async, cancellable
    public static List<(List<int> ExecutedThreads, List<string> Output)> Test(
        List<StartBlock> startBlocks,
        List<string> stdin)
    {
        var result = new List<(List<int>, List<string>)>();
        var queue = new Queue<(List<int> ExecutedThreads, List<int> WorkingThreads)>();
        queue.Enqueue(([], Enumerable.Range(0, startBlocks.Count).ToList()));
        while (queue.Count > 0)
        {
            var (executedThreads, workingThreads) = queue.Dequeue();
            foreach (var thread in workingThreads)
            {
                var visitors = ReplayOperations(startBlocks, executedThreads, stdin);
                var (visitor, stdout) = visitors[thread];
                visitor.Next();
                var nextExecutedThreads = executedThreads.Append(thread).ToList();
                var nextWorkingThreads = visitor.IsDone
                    ? workingThreads.Where(t => t != thread).ToList()
                    : [.. workingThreads];
                if (nextWorkingThreads.Count > 0)
                {
                    queue.Enqueue((nextExecutedThreads, nextWorkingThreads));
                }
                else
                {
                    // add to result on each iter?
                    result.Add((nextExecutedThreads, stdout.Lines));
                    Console.WriteLine($"done: sequence {string.Join(", ", nextExecutedThreads)}");
                }
            }
        }

        return result;
    }

    private static List<(InterpretingVisitor, MockStdout)> ReplayOperations(
        List<StartBlock> startBlocks,
        List<int> executedThreads,
        List<string> stdin)
    {
        var variables = new ConcurrentDictionary<string, int>();
        var result = new List<(InterpretingVisitor, MockStdout)>(startBlocks.Count);
        foreach (var block in startBlocks)
        {
            var stdout = new MockStdout();
            var visitor = new InterpretingVisitor(variables, stdout, new MockStdin(stdin));
            result.Add((visitor, stdout));
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