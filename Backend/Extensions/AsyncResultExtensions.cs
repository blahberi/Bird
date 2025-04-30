using Backend.Core;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs;
using Shared.Extensions;

namespace Backend.Extensions;

public static class AsyncResultExtensions
{
    public static async Task<IActionResult> ToActionResultAsync<T, E>(
        this Task<Result<T, E>> resultTask,
        Func<T, IActionResult> onSuccess,
        Func<E, IActionResult> onError)
    {
        Result<T, E> result = await resultTask;
        return result.Match(onSuccess, onError);
    }
    
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T, Error>> resultTask)
    {
        return await resultTask.ToActionResultAsync(
            onSuccess: value => new OkObjectResult(value),
            onError: error =>
            {
                return error.Type switch
                {
                    ErrorType.Unauthorized => new UnauthorizedObjectResult(new ErrorResponse
                    {
                        Error = error.Message, Details = error.Details
                    }),
                    _ => new BadRequestObjectResult(new ErrorResponse
                    {
                        Error = error.Message, Details = error.Details
                    })
                };
            }
        );
    }
    
    public static async Task<Result<T, Error>> OkIfAsync<T>(this Task<T?> valueTask, Func<T, bool> predicate, Error error)
    {
        return await valueTask.OkIfAsync(predicate, () => error);
    }
    
    public static async Task<Result<T, Error>> OkIfAsync<T>(this Task<T?> valueTask, Func<T, bool> predicate, string errorMessage)
    {
        return await valueTask.OkIfAsync(predicate, new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static async Task<Result<T, Error>> ErrIfAsync<T>(this Task<T?> valueTask, Func<T, bool> predicate, Error error)
    {
        return await valueTask.ErrIfAsync(predicate, () => error);
    }
    
    public static async Task<Result<T, Error>> ErrIfAsync<T>(this Task<T?> valueTask, Func<T, bool> predicate, string errorMessage)
    {
        return await valueTask.ErrIfAsync(predicate, new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static async Task<Result<T, Error>> ToResultAsync<T>(this Task<T?> valueTask, Error error)
    {
        return await valueTask.ToResultAsync(() => error);
    }
    
    public static async Task<Result<T, Error>> ToResultAsync<T>(this Task<T?> valueTask, string errorMessage)
    {
        return await valueTask.ToResultAsync(new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
    
    public static async Task<Result<T, Error>> ToOkResultAsync<T>(this Task<T> valueTask)
    {
        return await valueTask.ToOkResultAsync<T, Error>();
    }
    
    public static async Task<Result<IEnumerable<T>, Error>> ToEnumerableResultAsync<T>(this Task<IEnumerable<T>> valuesTask, Error error)
    {
        return await valuesTask.ToEnumerableResultAsync(() => error);
    }
    
    public static async Task<Result<IEnumerable<T>, Error>> ToEnumerableResultAsync<T>(this Task<IEnumerable<T>> valuesTask, string errorMessage)
    {
        return await valuesTask.ToEnumerableResultAsync(() => new Error(
            errorMessage, 
            new Dictionary<string, string>()));
    }
}