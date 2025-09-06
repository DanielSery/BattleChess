using FluentResults;

namespace CrownsGuard.Database;

public class CancelledError : IError
{
    public static readonly CancelledError Instance = new CancelledError();

    /// <inheritdoc />
    public string Message => "Operation cancelled";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new List<IError>();
}