namespace EmployeeStructureSystem.Application.Common;

public class OperationResult
{
    protected OperationResult(bool succeeded, string? errorMessage = null, string? errorCode = null, IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    public bool Succeeded { get; }

    public string? ErrorMessage { get; }

    public string? ErrorCode { get; }

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    public static OperationResult Success() => new(true);

    public static OperationResult Failure(string message, string? errorCode = null) => new(false, message, errorCode);

    public static OperationResult ValidationFailure(IReadOnlyDictionary<string, string[]> validationErrors, string? message = null)
        => new(false, message ?? "Validation failed.", "validation", validationErrors);
}

public sealed class OperationResult<T> : OperationResult
{
    private OperationResult(bool succeeded, T? value = default, string? errorMessage = null, string? errorCode = null, IReadOnlyDictionary<string, string[]>? validationErrors = null)
        : base(succeeded, errorMessage, errorCode, validationErrors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static OperationResult<T> Success(T value) => new(true, value);

    public static new OperationResult<T> Failure(string message, string? errorCode = null) => new(false, default, message, errorCode);

    public static new OperationResult<T> ValidationFailure(IReadOnlyDictionary<string, string[]> validationErrors, string? message = null)
        => new(false, default, message ?? "Validation failed.", "validation", validationErrors);
}