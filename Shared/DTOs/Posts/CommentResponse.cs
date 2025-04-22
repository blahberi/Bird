namespace Shared.DTOs.Posts;

public class CommentResponse : Response
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ContentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public int LikesCount { get; set; }
}