using FluentResults;

namespace CrownsGuard.Database.Errors;

public class ObjectIdParseError : IError
{
    public static readonly ObjectIdParseError Instance = new();

    /// <inheritdoc />
    public string Message => "Failed to parse ObjectId";

    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; } = new();

    /// <inheritdoc />
    public List<IError> Reasons { get; } =  new();
}