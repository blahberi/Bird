using Backend.Core;
using Backend.Services;
using Shared;
using Shared.Extensions;
namespace Backend.Extensions;

public static class PostsExtensions
{
    public static async Task<Result<Content, Error>> GetPostByIdAsync(this IPostsService postsService, int postId)
    {
        Result<Content, Error> result = await postsService.GetContentById(postId);
        return result.Match(
            onSuccess: content =>
            {
                if (content.ParentId != null)
                    return Error.CreateErr<Content>("Not a post");
                return Error.CreateOk(content);
            },
            onError: Result<Content, Error>.CreateErr
        );
    }

    public static async Task<Result<None, Error>> CreatePostAsync(this IPostsService postsService, Content post)
    {
        post.ParentId = null;
        return await postsService.CreateContentAsync(post);
    }

    public static async Task<Result<IEnumerable<Content>, Error>> GetAllPostsAsync(this IPostsService postsService, int page,
        int pageSize)
    {
        ContentFilter filter = content => content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<int, Error>> GetAllPostsCountAsync(this IPostsService postsService)
    {
        ContentFilter filter = content => content.ParentId == null;
        return await postsService.GetContentCountAsync(filter);
    }

    public static async Task<Result<IEnumerable<Content>, Error>> GetPostsByUserAsync(this IPostsService postsService, ContentOrder order, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<IEnumerable<Content>, Error>> GetPostsByUserAsync(this IPostsService postsService, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }
}
