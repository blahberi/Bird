using Shared;
using Shared.DTOs.Posts;
using Frontend.Core;
namespace Frontend.Services;

public interface ILikesService
{
    Task<Result<None, Error>> LikePostAsync(int postId);
    Task<Result<None, Error>> UnlikePostAsync(int postId);
    Task<Result<LikeCountResponse, Error>> GetLikesCountByPostIdAsync(int postId);
}