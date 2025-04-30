global using ContentFilter = System.Linq.Expressions.Expression<System.Func<Backend.Core.Content, bool>>;

using Backend.Core;
using Shared;

namespace Backend.Services;

public delegate IOrderedQueryable<Content> ContentOrder(IQueryable<Content> contents);
public interface IPostsService
{
    Task<Result<Content, Error>> GetContentById(int id);
    Task<Result<None, Error>> CreateContentAsync(Content content);
    Task<Result<IEnumerable<Content>, Error>> GetContentsAsync(ContentFilter filter, ContentOrder order, int page, int pageSize);
    Task<Result<int, Error>> GetContentCountAsync(ContentFilter filter);
}