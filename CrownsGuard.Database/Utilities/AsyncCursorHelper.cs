using CrownsGuard.Database.Errors;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Utilities;

public static class AsyncCursorHelper
{
    public static async Task<IChangeStreamCursor<ChangeStreamDocument<T>>> WatchAsync<T>(
        this IMongoCollection<T> collection, 
        FilterDefinition<ChangeStreamDocument<T>>? filter,
        CancellationToken cancellationToken)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>().Match(filter);
        return await collection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationToken
        );
    }
    
    public static async Task<Result<T>> WaitForUpdateAsync<T>(
        this IChangeStreamCursor<ChangeStreamDocument<T>> cursor, 
        CancellationToken cancellationToken)
    {
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var change in cursor.Current)
            {
                if (change.OperationType == ChangeStreamOperationType.Delete)
                    return Result.Fail("Deleted instead of update");
                
                return change.FullDocument;
            }
        }

        throw new OperationFailedException("Failed to wait for add");
    }
    
    public static async Task<T> WaitForAddAsync<T>(
        this IChangeStreamCursor<ChangeStreamDocument<T>> cursor, 
        CancellationToken cancellationToken)
    {
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
        if (!hasFirstBatch) return Result.Fail(NoResultsFoundError.Instance);

        using var firstBatch = cursor.Current.GetEnumerator();
        if (!firstBatch.MoveNext()) return Result.Fail(NoResultsFoundError.Instance);

        var current = firstBatch.Current;
        if (firstBatch.MoveNext()) return Result.Fail(TooManyResultsFoundError.Instance);

        var hasSecondBatch = await cursor.MoveNextAsync(cancellationToken);
        if (hasSecondBatch) return Result.Fail(TooManyResultsFoundError.Instance);
        
        return current;
    }
    
    public static async Task<Result<T>> SingleResultAsync<T>(this IAsyncCursor<T> cursor, CancellationToken cancellationToken)
    {
        var hasFirstBatch = await cursor.MoveNextAsync(cancellationToken);
        if (!hasFirstBatch) return Result.Fail(NoResultsFoundError.Instance);

        using var firstBatch = cursor.Current.GetEnumerator();
        if (!firstBatch.MoveNext()) return Result.Fail<T>(NoResultsFoundError.Instance);

        var current = firstBatch.Current;
        if (firstBatch.MoveNext()) return Result.Fail(TooManyResultsFoundError.Instance);

        var hasSecondBatch = await cursor.MoveNextAsync(cancellationToken);
        if (hasSecondBatch) return Result.Fail<T>(TooManyResultsFoundError.Instance);
        
        return Result.Ok(current);
    }
}