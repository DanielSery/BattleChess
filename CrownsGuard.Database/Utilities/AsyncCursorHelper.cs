using CrownsGuard.Database.Errors;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Utilities;

public static class AsyncCursorHelper
{
    public static async Task<T> WaitForAddAsync<T>(
        this IMongoCollection<T> collection, 
        FilterDefinition<ChangeStreamDocument<T>>? filter,
        CancellationToken cancellationToken)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>().Match(filter);

        using var cursor = await collection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationToken
        );
        
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var change in cursor.Current)
            {
                return change.FullDocument;
            }
        }

        throw new OperationFailedException("Failed to wait for add");
    }
    
    public static async Task<Result<T>> FindSingleResultAsync<T>(
        this IMongoCollection<T> collection, 
        FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        var cursor = await collection.FindAsync(filter, cancellationToken: cancellationToken);
        
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