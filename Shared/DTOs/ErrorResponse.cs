namespace Shared.DTOs
{
    public class ErrorResponse : Response
    {
        public string Error { get; set; } = string.Empty;
        public IDictionary<string, string> Details { get; set; } = new Dictionary<string, string>();
    }
}
