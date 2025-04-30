namespace Shared.DTOs.Posts;

public class ListPostResponse : Response
{
    public IEnumerable<PostResponse> Posts { get; set; } = new List<PostResponse>();
    public int TotalCount { get; set; }
}