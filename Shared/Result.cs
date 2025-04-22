using Shared.Extensions;

namespace Shared
{
    public class Result
    {
        private readonly string? error;
        private readonly IDictionary<string, string>? errors;

        protected Result(bool success, string? error, IDictionary<string, string>? errors)
        {
            this.Success = success;
            this.error = error;
            this.errors = errors;
        }

        public bool Success { get; }
        

        public string? Error
        {
            get
            {
                if (this.Success)
                {
                    throw new InvalidOperationException("ErrorMessage is invalid when result is successful");
                }

                return this.error;
            }
        }

        public IDictionary<string, string>? Errors
        {
            get
            {
                if (this.Success)
                {
                    throw new InvalidOperationException("Errors is invalid when result is successful");
                }

                return this.errors;
            }
        }

        public static Result SuccessResult()
        {
            return new Result(true, error: null, errors: null);
        }

        public static Result FailureResult(string error)
        {
            return new Result(false, error, errors: null);
        }

        public static Result FailureResult(IDictionary<string, string> errors)
        {
            return new Result(false, error: null, errors);
        }
    }

    public class Result<T> : Result
    {
        private readonly T? value;

        private Result(bool success, T? value, string? error, IDictionary<string, string>? errors)
            : base(success, error, errors)
        {
            this.value = value;
        }

        public T? Value
        {
            get
            {
                if (!this.Success)
                {
                    throw new InvalidOperationException("Value is invalid when result is non successful");
                }

                return this.value;
            }
        }

        public static Result<T> SuccessResult(T value)
        {
            return new Result<T>(true, value, error: null, errors: null);
        }

        public static new Result<T> FailureResult(string error)
        {
            return new Result<T>(false, value: default, error, errors: null);
        }

        public static new Result<T> FailureResult(IDictionary<string, string> errors)
        {
            return new Result<T>(false, value: default, error: null, errors);
        }
    }

}
