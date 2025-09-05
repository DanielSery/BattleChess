using System.Diagnostics.CodeAnalysis;
using FluentResults;

namespace CrownsGuard.Multiplayer.Utilities;

public static class ResultHelper
{
    public static bool TryGetValue<T>(this Result<T> result, [MaybeNullWhen(false)] out T successValue)
    {
        if (result.IsSuccess)
        {
            successValue = result.Value;
            return true;
        }

        successValue = default;
        return false;
    }
}