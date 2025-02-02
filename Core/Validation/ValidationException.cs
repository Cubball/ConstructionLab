namespace Core.Validation;

public sealed class ValidationException(string message) : Exception(message);