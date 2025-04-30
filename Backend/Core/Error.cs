using Shared;

namespace Backend.Core;

public class Error
{
    public string Message { get; set; }
    public IDictionary<string, string> Details { get; set; } 
    public ErrorType Type { get; set; }
    
    public Error(string message, IDictionary<string, string> details, ErrorType type)
    {
        this.Message = message;
        this.Details = details;
        this.Type = type;
    }
    
    public Error(string message, IDictionary<string, string> details)
    {
        this.Message = message;
        this.Details = details;
        this.Type = ErrorType.BadRequest;
    }
    
    public static Result<T, Error> CreateOk<T>(T value)
    {
        return Result<T, Error>.CreateOk(value);
    }
    
    public static Result<T, Error> CreateErr<T>(string message)
    {
        return Result<T, Error>.CreateErr(new Error(
            message,
            new Dictionary<string, string>()));
    }
    
    public static Result<T, Error> CreateErr<T>(string message, IDictionary<string, string> details)
    {
        return Result<T, Error>.CreateErr(new Error(
            message,
            details));
    }
}

public enum ErrorType
{
    BadRequest,
    Unauthorized,
}