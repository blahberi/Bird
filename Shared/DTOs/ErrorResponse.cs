namespace Shared.DTOs
{
    public class ErrorResponse : Response
    {
        public string? Error { get; set; }
        public IDictionary<string, string>? Details { get; set; }
    }
}
