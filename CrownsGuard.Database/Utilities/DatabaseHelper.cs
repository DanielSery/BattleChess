using CrownsGuard.Database.Errors;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CrownsGuard.Database.Utilities;

public static class DatabaseHelper
{
    /// <summary>
    /// Executes an async operation with standardized error handling
    /// </summary>
    public static async Task<Result> ExecuteWithErrorHandling(
        Func<Task> operation, string operationName, ILogger logger, CancellationToken manualToken)
    {
        try
        {
            logger.LogInformation("Started {OperationName}", operationName);
            await operation();
            logger.LogInformation("Finished {OperationName}", operationName);
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
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", operationName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", operationName);
            return Result.Fail(e.Message);
        }
    }
    
    /// <summary>
    /// Executes an async operation with standardized error handling
    /// </summary>
    public static async Task<Result> ExecuteWithErrorHandling(
        Func<Task<Result>> operation, string operationName, ILogger logger, CancellationToken manualToken)
    {
        try
        {
            logger.LogInformation("Started {OperationName}", operationName);
            var result = await operation();
            if (result.IsSuccess)
            {
                logger.LogInformation("Finished {OperationName}", operationName);
            }
            else
            {
                logger.LogError("Failed {OperationName} with error {Errors}", operationName, string.Join(", ", result.Errors));
            }

            return result;
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
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", operationName);
            return Result.Fail(ObjectIdParseError.Instance);
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
        Func<Task<Result<T>>> operation, string operationName, ILogger logger, CancellationToken manualToken)
    {
        try
        {
            logger.LogInformation("Started {OperationName}", operationName);
            var result = await operation();
            logger.LogInformation("Finished {OperationName}", operationName);
            if (result.IsSuccess)
            {
                logger.LogInformation("Finished {OperationName}", operationName);
            }
            else
            {
                logger.LogError("Failed {OperationName} with error {Errors}", operationName, string.Join(", ", result.Errors));
            }
            return result;
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
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", operationName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", operationName);
            return Result.Fail<T>(e.Message);
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
            logger.LogInformation("Started {OperationName}", operationName);
            var result = await operation();
            logger.LogInformation("Finished {OperationName}", operationName);
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
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", operationName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", operationName);
            return Result.Fail<T>(e.Message);
        }
    }
}