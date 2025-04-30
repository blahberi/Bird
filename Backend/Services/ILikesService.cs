global using LikeFilter = System.Linq.Expressions.Expression<System.Func<Backend.Core.Like, bool>>;

using Backend.Core;
using Shared;

namespace Backend.Services;

public delegate IOrderedQueryable<Like> LikeOrder(IQueryable<Like> likes);

public interface ILikesService
{
    Task<Result<None, Error>> LikeContentAsync(int contentId, int userId);
    Task<Result<None, Error>> UnlikeContentAsync(int contentId, int userId);
    Task<Result<bool, Error>> HasUserLikedContentAsync(int contentId, int userId);
    Task<Result<IEnumerable<bool>, Error>> HasUserLikedContentsAsync(int userId, IEnumerable<int> contentIds);
    Task<Result<IEnumerable<Like>, Error>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize);
    Task<Result<int, Error>> GetLikesCountAsync(LikeFilter filter);
} 