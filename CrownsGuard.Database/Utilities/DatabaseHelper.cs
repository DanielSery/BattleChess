using System.Runtime.CompilerServices;
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
        ILogger logger, 
        CancellationToken manualToken,
        int timeoutSeconds, 
        Func<CancellationToken, Task> operation, 
        [CallerMemberName] string callerName = "")
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(manualToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            logger.LogInformation("Started {OperationName}", callerName);
            await operation(linkedSource.Token);
            logger.LogInformation("Finished {OperationName}", callerName);
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", callerName);
                return Result.Fail(CancelledError.Instance);
            }
            
            logger.LogWarning("Operation {OperationName} was timed out", callerName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", callerName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", callerName);
            return Result.Fail(e.Message);
        }
    }
    
    /// <summary>
    /// Executes an async operation with standardized error handling
    /// </summary>
    public static async Task<Result> ExecuteWithErrorHandling(
        ILogger logger, 
        CancellationToken manualToken,
        int timeoutSeconds, 
        Func<CancellationToken, Task<Result>> operation, 
        [CallerMemberName] string callerName = "")
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(manualToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            logger.LogInformation("Started {OperationName}", callerName);
            var result = await operation(linkedSource.Token);
            if (result.IsSuccess)
            {
                logger.LogInformation("Finished {OperationName}", callerName);
            }
            else
            {
                logger.LogError("Failed {OperationName} with error {Errors}", callerName, string.Join(", ", result.Errors));
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", callerName);
                return Result.Fail(CancelledError.Instance);
            }
            
            logger.LogWarning("Operation {OperationName} was timed out", callerName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", callerName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", callerName);
            return Result.Fail(e.Message);
        }
    }

    /// <summary>
    /// Executes an async operation with standardized error handling and return value
    /// </summary>
    public static async Task<Result<T>> ExecuteWithErrorHandling<T>(
        ILogger logger, 
        CancellationToken manualToken,
        int timeoutSeconds, 
        Func<CancellationToken, Task<Result<T>>> operation, 
        [CallerMemberName] string callerName = "")
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(manualToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            logger.LogInformation("Started {OperationName}", callerName);
            var result = await operation(linkedSource.Token);
            logger.LogInformation("Finished {OperationName}", callerName);
            if (result.IsSuccess)
            {
                logger.LogInformation("Finished {OperationName}", callerName);
            }
            else
            {
                logger.LogError("Failed {OperationName} with error {Errors}", callerName, string.Join(", ", result.Errors));
            }
            return result;
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", callerName);
                return Result.Fail(CancelledError.Instance);
            }

            logger.LogWarning("Operation {OperationName} was timed out", callerName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", callerName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", callerName);
            return Result.Fail<T>(e.Message);
        }
    }

    /// <summary>
    /// Executes an async operation with standardized error handling and return value
    /// </summary>
    public static async Task<Result<T>> ExecuteWithErrorHandling<T>(
        ILogger logger, 
        CancellationToken manualToken, 
        int timeoutSeconds, 
        Func<CancellationToken, Task<T>> operation, 
        [CallerMemberName] string callerName = "")
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(manualToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            logger.LogInformation("Started {OperationName}", callerName);
            var result = await operation(linkedSource.Token);
            logger.LogInformation("Finished {OperationName}", callerName);
            return Result.Ok(result);
        }
        catch (OperationCanceledException)
        {
            if (manualToken.IsCancellationRequested)
            {
                logger.LogWarning("Operation {OperationName} was cancelled", callerName);
                return Result.Fail(CancelledError.Instance);
            }

            logger.LogWarning("Operation {OperationName} was timed out", callerName);
            return Result.Fail(TimeoutError.Instance);
        }
        catch (FormatException e)
        {
            logger.LogError(e, "Invalid object format in operation {OperationName}", callerName);
            return Result.Fail(ObjectIdParseError.Instance);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to execute operation {OperationName}", callerName);
            return Result.Fail<T>(e.Message);
        }
    }
}