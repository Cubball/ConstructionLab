namespace Core.Testing;

public record TestingResult(Dictionary<int, double> SuccessRates, int MaxExecutedSteps);