using System.Collections.Concurrent;
using Core.Interpretation;
using Core.Models;

namespace Core.Testing;

internal class Tester
{
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
                var newExecutedThreads = executedThreads.Append(thread).ToList();
                List<int> newWorkingThreads;
                if (visitor.IsDone)
                {
                    Console.WriteLine($"thread {thread} done: sequence {string.Join(", ", newExecutedThreads)}");
                    newWorkingThreads = workingThreads.Where(t => t != thread).ToList();
                }
                else
                {
                    newWorkingThreads = [.. workingThreads];
                }

                if (workingThreads.Count > 0)
                {
                    queue.Enqueue((newExecutedThreads, newWorkingThreads));
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
            var signal = new AutoResetEvent(false);
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