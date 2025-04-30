global using ContentFilter = System.Linq.Expressions.Expression<System.Func<Backend.Core.Content, bool>>;

using Backend.Core;
using Shared;

namespace Backend.Services;

public delegate IOrderedQueryable<Content> ContentOrder(IQueryable<Content> contents);
public interface IPostsService
{
    Task<Content> GetContentById(int id);
    Task CreateContentAsync(Content content);
    Task<IEnumerable<Content>> GetContentsAsync(ContentFilter filter, ContentOrder order, int page, int pageSize);
    Task<int> GetContentCountAsync(ContentFilter filter);
}