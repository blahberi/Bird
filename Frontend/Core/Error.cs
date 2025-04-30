using Shared;

namespace Frontend.Core;

public class Error
{
    public string Message { get; set; }
    public IDictionary<string, string> Details { get; set; }

    public Error(string message, IDictionary<string, string> details)
    {
        this.Message = message;
        this.Details = details;
    }

    public static Result<T, Error> CreateOkResult<T>(T value)
    {
        return Result<T, Error>.CreateOk(value);
    }

    public static Result<T, Error> CreateErrResult<T>(string message)
    {
        return Result<T, Error>.CreateErr(new Error(message, new Dictionary<string, string>()));
    }

    public static Result<T, Error> CreateErrResult<T>(string message, IDictionary<string, string> details)
    {
        return Result<T, Error>.CreateErr(new Error(message, details));
    }
}