using Backend.Core;
using Backend.Services;
using Shared;
using Shared.Extensions;
namespace Backend.Extensions;

public static class PostsExtensions
{
    public static async Task<Content> GetContentByIdAsync(this IPostsService postsService, int postId)
    {
        Content result = await postsService.GetContentById(postId);
        if (result.ParentId != null)
        {
            throw new Exception("Not a post");
        }
        return result;
    }

    public static async Task<None> CreatePostAsync(this IPostsService postsService, Content post)
    {
        post.ParentId = null;
        await postsService.CreateContentAsync(post);
        return None.Value;
    }

    public static async Task<IEnumerable<Content>> GetAllPostsAsync(this IPostsService postsService, int page,
        int pageSize)
    {
        ContentFilter filter = content => content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<int> GetAllPostsCountAsync(this IPostsService postsService)
    {
        ContentFilter filter = content => content.ParentId == null;
        return await postsService.GetContentCountAsync(filter);
    }

    public static async Task<IEnumerable<Content>> GetPostsByUserAsync(this IPostsService postsService, ContentOrder order, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<IEnumerable<Content>> GetPostsByUserAsync(this IPostsService postsService, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }
}
