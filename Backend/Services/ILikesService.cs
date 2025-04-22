global using LikeFilter = System.Linq.Expressions.Expression<System.Func<Backend.Core.Like, bool>>;

using Backend.Core;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Backend.Services;

public delegate IOrderedQueryable<Like> LikeOrder(IQueryable<Like> likes);

public interface ILikesService
{
    Task<Result> LikeContentAsync(int contentId, int userId);
    Task<Result> UnlikeContentAsync(int contentId, int userId);
    Task<Result<bool>> HasUserLikedContentAsync(int contentId, int userId);
    Task<Result<List<Like>>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize);
    Task<Result<int>> GetLikesCountAsync(LikeFilter filter);
} 