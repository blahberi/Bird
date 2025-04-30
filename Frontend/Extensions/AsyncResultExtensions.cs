using Shared;
using Shared.Extensions;
using Frontend.Core;
namespace Frontend.Extensions;

public static class AsyncResultExtensions
{
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
