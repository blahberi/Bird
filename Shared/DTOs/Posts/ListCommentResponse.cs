namespace Shared.DTOs.Posts;

public class ListCommentResponse : Response
{
    public IEnumerable<CommentResponse> Comments { get; set; } = new List<CommentResponse>();
    public int TotalCount { get; set; }
}