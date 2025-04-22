using Backend.Core;
using Backend.Services;
using Shared;

namespace Backend.Extensions;

public static class LikesExtensions
{
    public static async Task<Result<List<Like>>> GetLikesByContentIdAsync(this ILikesService likesService, int contentId, int page, int pageSize)
    {
        LikeFilter filter = like => like.ContentId == contentId;
        LikeOrder order = likes => likes.OrderByDescending(l => l.CreatedAt);
        return await likesService.GetLikesAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<int>> GetLikesCountByContentIdAsync(this ILikesService likesService, int contentId)
    {
        LikeFilter filter = like => like.ContentId == contentId;
        return await likesService.GetLikesCountAsync(filter);
    }

    public static async Task<Result<List<Like>>> GetLikesByUserIdAsync(this ILikesService likesService, int userId, int page, int pageSize)
    {
        LikeFilter filter = like => like.UserId == userId;
        LikeOrder order = likes => likes.OrderByDescending(l => l.CreatedAt);
        return await likesService.GetLikesAsync(filter, order, page, pageSize);
    }
}