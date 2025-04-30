namespace Shared.Extensions;

public static class ResultsExtensions
{
    public static Result<U, E> Map<T, U, E>(this Result<T, E> result, Func<T, U> mapper)
    {
        return result switch
        {
            Result<T, E>.Ok ok => Result<U, E>.CreateOk(mapper(ok.Value)),
            Result<T, E>.Err err => Result<U, E>.CreateErr(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }


    public static T DefaultIfError<T, E>(this Result<T, E> result, T defaultValue)
    {
        return result is Result<T, E>.Ok ok ? ok.Value : defaultValue;
    }

    public static Result<T, E> LogError<T, E>(this Result<T, E> result, Action<E> logger)
    {
        if (result is Result<T, E>.Err err)
        {
            logger(err.Error);
        }
        return result;
    }

    public static Result<IEnumerable<T>, IEnumerable<E>> Combine<T, E>(this IEnumerable<Result<T, E>> results)
    {
        var successes = new List<T>();
        var errors = new List<E>();

        foreach (var result in results)
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
    
    public static Result<T, E> OkIf<T, E>(this T? value , Func<T, bool> predicate, Func<E> errorFunc)
    {
        return value != null && predicate(value) ? Result<T, E>.CreateOk(value) : Result<T, E>.CreateErr(errorFunc());
    }

    public static Result<T, E> ErrIf<T, E>(this T? value, Func<T, bool> predicate, Func<E> errorFunc)
    {
        return value.OkIf(v => !predicate(v), errorFunc);
    }

    public static Result<T, E> ToResult<T, E>(this T? value, Func<E> errorFunc)
    {
        return value.OkIf(_ => true, errorFunc);
    }

    public static Result<T, E> ToOkResult<T, E>(this T value)
    {
        return Result<T, E>.CreateOk(value);
    }

    public static Result<IEnumerable<T>, E> ToEnumerableResult<T, E>(this IEnumerable<T> values, Func<E> errorFunc)
    {
        return values.OkIf(v => v.Any(), errorFunc);
    }

    public static Result<T, F> MapErr<T, E, F>(this Result<T, E> result, Func<E, F> mapper)
    {
        return result switch
        {
            Result<
            T, E>.Ok ok => Result<T, F>.CreateOk(ok.Value),
            Result<T, E>.Err err => Result<T, F>.CreateErr(mapper(err.Error)),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static TResult Match<T, E, TResult>(
        this Result<T, E> result,
        Func<T, TResult> onSuccess,
        Func<E, TResult> onError)
    {
        return result switch
        {
            Result<T, E>.Ok ok => onSuccess(ok.Value),
            Result<T, E>.Err err => onError(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static Result<U, E> AndThen<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> binder)
    {
        return result switch
        {
            Result<T, E>.Ok ok => binder(ok.Value),
            Result<T, E>.Err err => Result<U, E>.CreateErr(err.Error),
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

    public static Result<(T, U), E> AndThenWith<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> binder)
    {
        return result
            .AndThen(value =>
            {
                return binder(value).Map(u => (value, u));
            });
    }

    public static Result<T, E> Ensure<T, E>(this Result<T, E> result, Func<T, bool> predicate, Func<E> errorFunc)
    {
        return result switch
        {
            Result<T, E>.Ok ok when predicate(ok.Value) => result,
            Result<T, E>.Ok => Result<T, E>.CreateErr(errorFunc()),
            Result<T, E>.Err err => err,
            _ => throw new InvalidOperationException("Unexpected Result state")
        };
    }

}