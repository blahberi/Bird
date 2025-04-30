global using LikeFilter = System.Linq.Expressions.Expression<System.Func<Backend.Core.Like, bool>>;

using Backend.Core;
using Shared;

namespace Backend.Services;

public delegate IOrderedQueryable<Like> LikeOrder(IQueryable<Like> likes);

public interface ILikesService
{
    Task LikeContentAsync(int contentId, int userId);
    Task UnlikeContentAsync(int contentId, int userId);
    Task<bool> HasUserLikedContentAsync(int contentId, int userId);
    Task<IEnumerable<Like>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize);
    Task<int> GetLikesCountAsync(LikeFilter filter);
} 