using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public interface ILikesService
{
    Task<Result> LikePostAsync(int postId);
    Task<Result> UnlikePostAsync(int postId);
    Task<Result<LikeCountResponse>> GetLikesCountByPostIdAsync(int postId);
}