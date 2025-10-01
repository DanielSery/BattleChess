using FluentResults;

namespace CrownsGuard.Database.Errors;

public class NoResultsFoundError : IError
{
    public static readonly NoResultsFoundError Instance = new();

    /// <inheritdoc />
    public string Message => "No results found";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new();
}