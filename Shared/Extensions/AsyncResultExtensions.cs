namespace Shared.Extensions;

public static class AsyncResultExtensions
{
    public static async Task<Result<U, E>> MapAsync<T, U, E>(this Task<Result<T, E>> result, Func<T, Task<U>> mapper)
    {
        return result switch
        {
            Result<T, E>.Ok ok => Result<U, E>.CreateOk(await mapper(ok.Value)),
            Result<T, E>.Err err => Result<U, E>.CreateErr(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static async Task<Result<T, E>> LogErrorAsync<T, E>(this Task<Result<T, E>> resultTask, Action<E> logger)
    {
        Result<T, E> result = await resultTask;
        if (result is Result<T, E>.Err err)
        {
            logger(err.Error);
        }
        return result;
    }

    public static async Task<Result<IEnumerable<T>, IEnumerable<E>>> CombineAsync<T, E>(this Task<IEnumerable<Result<T, E>>> resultsTask)
    {
        IEnumerable<Result<T, E>> results = await resultsTask;
        List<T> successes = new List<T>();
        List<E> errors = new List<E>();

        foreach (Result<T, E> result in results)
        {
            if (result is Result<T, E>.Ok ok)
                successes.Add(ok.Value);
            else if (result is Result<T, E>.Err err)
                errors.Add(err.Error);
        }

        return errors.Any()
            ? Result<IEnumerable<T>, IEnumerable<E>>.CreateErr(errors)
            : Result<IEnumerable<T>, IEnumerable<E>>.CreateOk(successes);
    }
    
    public static async Task<Result<T, E>> OkIfAsync<T, E>(this Task<T?> valueTask, Func<T, bool> predicate, Func<E> errorFunc)
    {
        T? value = await valueTask;
        return value != null && predicate(value) ? Result<T, E>.CreateOk(value) : Result<T, E>.CreateErr(errorFunc());
    }
    
    public static async Task<Result<T, E>> ErrIfAsync<T, E>(this Task<T?> valueTask, Func<T, bool> predicate, Func<E> errorFunc)
    {
        return await valueTask.OkIfAsync(v => !predicate(v), errorFunc);
    }
    
    public static async Task<Result<T, E>> ToResultAsync<T, E>(this Task<T?> valueTask, Func<E> errorFunc)
    {
        return await valueTask.OkIfAsync(_ => true, errorFunc);
    }

    public static async Task<Result<T, E>> ToOkResultAsync<T, E>(this Task<T> valueTask)
    {
        T value = await valueTask;
        return Result<T, E>.CreateOk(value);
    }

    public static async Task<Result<IEnumerable<T>, E>> ToEnumerableResultAsync<T, E>(this Task<IEnumerable<T>> valuesTask, Func<E> errorFunc)
    {
        return await valuesTask!.OkIfAsync(v => v.Any(), errorFunc);
    }

    public static async Task<Result<T, F>> MapErrAsync<T, E, F>(this Task<Result<T, E>> resultTask, Func<E, F> mapper)
    {
        Result<T, E> result = await resultTask;
        return result switch
        {
            Result<T, E>.Ok ok => Result<T, F>.CreateOk(ok.Value),
            Result<T, E>.Err err => Result<T, F>.CreateErr(mapper(err.Error)),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static async Task<TResult> MatchAsync<T, E, TResult>(
        this Task<Result<T, E>> resultTask,
        Func<T, Task<TResult>> onSuccess,
        Func<E, Task<TResult>> onError)
    {
        Result<T, E> result = await resultTask;
        return result switch
        {
            Result<T, E>.Ok ok => await onSuccess(ok.Value),
            Result<T, E>.Err err => await onError(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static async Task<Result<U, E>> AndThenAsync<T, U, E>(this Task<Result<T, E>> resultTask, Func<T, Task<Result<U, E>>> binder)
    {
        Result<T, E> result = await resultTask;
        return result switch
        {
            Result<T, E>.Ok ok => await binder(ok.Value),
            Result<T, E>.Err err => Result<U, E>.CreateErr(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static async Task<Result<(T, U), E>> AndThenWithAsync<T, U, E>(this Task<Result<T, E>> resultTask,
        Func<T, Task<Result<U, E>>> binder)
    {
        return await resultTask
            .AndThenAsync(async value =>
            {
                return await binder(value).MapAsync(async u => (value, u));
            });
    }

    public static async Task<Result<T, E>> EnsureAsync<T, E>(this Task<Result<T, E>> resultTask, Func<T, bool> predicate, Func<E> errorFunc)
    {
        Result<T, E> result = await resultTask;
        return result switch
        {
            Result<T, E>.Ok ok when predicate(ok.Value) => result,
            Result<T, E>.Ok => Result<T, E>.CreateErr(errorFunc()),
            Result<T, E>.Err err => err,
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }
}
