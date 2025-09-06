using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Utilities;

public static class AsyncCursorHelper
{
    public static async Task<Result<T>> SingleResultAsync<T>(this IAsyncCursor<T> cursor, CancellationToken cancellationToken)
    {
        var hasFirstBatch = await cursor.MoveNextAsync(cancellationToken);
        if (!hasFirstBatch) return Result.Fail<T>("Could not find first element");

        using var firstBatch = cursor.Current.GetEnumerator();
        if (!firstBatch.MoveNext()) return Result.Fail<T>("Could not find first element");

        var current = firstBatch.Current;
        if (firstBatch.MoveNext()) return Result.Fail("Found second element");

        var hasSecondBatch = await cursor.MoveNextAsync(cancellationToken);
        if (hasSecondBatch) return Result.Fail<T>("Found second batch");
        
        return Result.Ok(current);
    }
}