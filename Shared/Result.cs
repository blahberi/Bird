namespace Shared;

public abstract class Result<T, E>
{
    public sealed class Ok : Result<T, E>
    {
        public T Value { get; }
        public Ok(T value) => Value = value;
    }

    public sealed class Err : Result<T, E>
    {
        public E Error { get; }
        public Err(E error) => Error = error;
    }

    public bool IsOk => this is Ok;
    public bool IsErr => this is Err;

    public T Value => this is Ok ok ? ok.Value : throw new InvalidOperationException("Result is not Ok");
    public E Error => this is Err err ? err.Error : throw new InvalidOperationException("Result is not Err");

    public static Result<T, E> CreateOk(T value) => new Ok(value);
    public static Result<T, E> CreateErr(E error) => new Err(error);
}