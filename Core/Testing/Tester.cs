using System.Collections.Concurrent;
using Core.Interpretation;
using Core.Models;
using Core.Validation;

namespace Core.Testing;

internal record struct ExecutionResult(List<int> ExecutedThreads, List<string> Output);

internal record struct ExecutionState(List<int> ExecutedThreads, List<int> WorkingThreads);

internal record struct ThreadsState(List<InterpretingVisitor> Visitor, MockStdout Output);

public static class Tester
{
    public static async Task<TestingResult> Test(
        List<StartBlock> startBlocks,
        List<string> stdout,
        List<string> stdin,
        CancellationToken cancellationToken = default)
    {
        Validator.Validate(startBlocks);
        var executionResults = await Task.Run(() => Test(startBlocks, stdin, cancellationToken));
        return AnalyzeExecutionResults(executionResults, stdout);
    }

    private static List<ExecutionResult> Test(
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

                var (visitors, stdout) = ReplayOperations(startBlocks, executedThreads, stdin);
                var visitor = visitors[thread];
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

    private static ThreadsState ReplayOperations(
        List<StartBlock> startBlocks,
        List<int> executedThreads,
        List<string> stdin)
    {
        var variables = new ConcurrentDictionary<string, int>();
        var visitors = new List<InterpretingVisitor>(startBlocks.Count);
        var mockStdout = new MockStdout();
        var mockStdin = new MockStdin(stdin);
        foreach (var block in startBlocks)
        {
            var visitor = new InterpretingVisitor(variables, mockStdout, mockStdin);
            visitors.Add(visitor);
            block.Accept(visitor);
        }

        foreach (var thread in executedThreads)
        {
            var visitor = visitors[thread];
            visitor.Next();
        }

        return new(visitors, mockStdout);
    }

    private static TestingResult AnalyzeExecutionResults(List<ExecutionResult> executionResults, List<string> expected)
    {
        var totalCounts = new Dictionary<int, int>();
        var successCounts = new Dictionary<int, int>();
        var maxExecutedSteps = 0;
        foreach (var executionResult in executionResults)
        {
            var (executedThreads, output) = executionResult;
            var executedSteps = executedThreads.Count;
            maxExecutedSteps = Math.Max(maxExecutedSteps, executedSteps);
            totalCounts[executedSteps] = totalCounts.GetValueOrDefault(executedSteps) + 1;
            if (output.SequenceEqual(expected))
            {
                successCounts[executedSteps] = successCounts.GetValueOrDefault(executedSteps) + 1;
            }
        }

        for (var i = 1; i < maxExecutedSteps; i++)
        {
            for (var j = i + 1; j <= maxExecutedSteps; j++)
            {
                totalCounts[j] = totalCounts.GetValueOrDefault(j) + totalCounts.GetValueOrDefault(i);
                successCounts[j] = successCounts.GetValueOrDefault(j) + successCounts.GetValueOrDefault(i);
            }
        }

        var result = new Dictionary<int, double>();
        for (var i = 1; i <= maxExecutedSteps; i++)
        {
            result[i] = successCounts.GetValueOrDefault(i) * 100.0 / totalCounts[i];
        }

        return new(result, maxExecutedSteps);
    }
}