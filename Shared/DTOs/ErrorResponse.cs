namespace Shared.DTOs
{
    public class ErrorResponse : Response
    {
        public string? Error { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
    }
}
