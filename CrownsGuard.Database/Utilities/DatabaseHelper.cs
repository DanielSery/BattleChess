using CrownsGuard.Database.Errors;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CrownsGuard.Database.Utilities;

public static class DatabaseHelper
{
    /// <summary>
    /// Safely parses an ObjectId with proper error handling
    /// </summary>
    public static ObjectId ParseObjectId(string objectIdValue, string fieldName, ILogger logger)
    {
        try
        {
            return ObjectId.Parse(objectIdValue);
        }
        catch (FormatException)
        {
            logger.LogWarning("Invalid {FieldName} ObjectId format: {ObjectIdValue}", fieldName, objectIdValue);
            throw new ObjectIdParseException();
        }
    }
    
    /// <summary>
    /// Executes an async operation with standardized error handling
    /// </summary>
    public static async Task<Result> ExecuteWithErrorHandling(
        Func<Task> operation, string operationName, ILogger logger, CancellationToken manualToken)
    {
        try
        {
            await operation();
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", operationName);
                return Result.Fail(CancelledError.Instance);
            }
            
            logger.LogWarning("Operation {OperationName} was timed out", operationName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", operationName);
            return Result.Fail(e.Message);
        }
    }

    /// <summary>
    /// Executes an async operation with standardized error handling and return value
    /// </summary>
    public static async Task<Result<T>> ExecuteWithErrorHandling<T>(
        Func<Task<T>> operation, string operationName, ILogger logger, CancellationToken manualToken)
    {
        try
        {
            var result = await operation();
            return Result.Ok(result);
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", operationName);
                return Result.Fail(CancelledError.Instance);
            }

            logger.LogWarning("Operation {OperationName} was timed out", operationName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (ObjectIdParseException)
        {
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", operationName);
            return Result.Fail<T>(e.Message);
        }
    }
}