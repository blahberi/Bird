namespace Shared.DTOs.Posts;

public class ListLikeResponse : Response
{
    public List<LikeResponse> Likes { get; set; } = new();
    public int TotalCount { get; set; }
}