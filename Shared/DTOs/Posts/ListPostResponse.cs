namespace Shared.DTOs.Posts;

public class ListPostResponse : Response
{
    public List<PostResponse> Posts { get; set; } = new();
    public int TotalCount { get; set; }
}