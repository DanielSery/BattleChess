using FluentResults;

namespace CrownsGuard.Database.Errors;

public class CancelledError : IError
{
    public static readonly CancelledError Instance = new();

    /// <inheritdoc />
    public string Message => "Operation cancelled";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new();
}