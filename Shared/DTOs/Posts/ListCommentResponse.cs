namespace Shared.DTOs.Posts;

public class ListCommentResponse : Response
{
    public List<CommentResponse> Comments { get; set; } = new();
    public int TotalCount { get; set; }
}