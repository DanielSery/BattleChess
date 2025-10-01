using FluentResults;

namespace CrownsGuard.Database.Errors;

public class TooManyResultsFoundError : IError
{
    public static readonly TooManyResultsFoundError Instance = new();

    /// <inheritdoc />
    public string Message => "Too many results found";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new();
}