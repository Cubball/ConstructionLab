using System.Collections.Concurrent;
using Core.Interpretation;
using Core.Models;

namespace Core.Testing;

internal class Tester
{
    // TODO:
    // have a list with 3 items in each tuple: sequence, in, out, return it afterwards
    // also, make this method cancellable

    // TODO: async or what?
    public static void Test(List<StartBlock> startBlocks, TextWriter @out, TextReader @in)
    {
        var queue = new Queue<(List<int> ExecutedThreads, List<int> WorkingThreads)>();
        queue.Enqueue(([], Enumerable.Range(0, startBlocks.Count).ToList()));
        while (queue.Count > 0)
        {
            var (executedThreads, workingThreads) = queue.Dequeue();
            foreach (var thread in workingThreads)
            {
                var visitors = ReplayOperations(startBlocks, executedThreads, @out, @in);
                var visitor = visitors[thread];
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
                    Console.WriteLine($"done: sequence {string.Join(", ", nextExecutedThreads)}");
                }
            }
        }
    }

    private static List<InterpretingVisitor> ReplayOperations(
        List<StartBlock> startBlocks,
        List<int> executedThreads,
        TextWriter @out,
        TextReader @in)
    {
        var variables = new ConcurrentDictionary<string, int>();
        var result = new List<InterpretingVisitor>(startBlocks.Count);
        foreach (var block in startBlocks)
        {
            var visitor = new InterpretingVisitor(variables, @out, @in);
            result.Add(visitor);
            block.Accept(visitor);
        }

        foreach (var thread in executedThreads)
        {
            var visitor = result[thread];
            visitor.Next();
        }

        return result;
    }
}