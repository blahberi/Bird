namespace Frontend.Models;

public class CommentModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public int LikesCount { get; set; }
}
