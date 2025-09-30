using FluentResults;

namespace CrownsGuard.Database.Errors;

public class TimeoutError : IError
{
    public static readonly TimeoutError Instance = new();

    /// <inheritdoc />
    public string Message => "Operation timeout";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new();
}