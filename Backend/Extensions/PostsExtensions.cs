using Backend.Core;
using Backend.Services;
using Shared;
using Shared.Extensions;
namespace Backend.Extensions;

public static class PostsExtensions
{
    public static async Task<Result<Content>> GetPostByIdAsync(this IPostsService postsService, int postId)
    {
        Result<Content> result = await postsService.GetContentById(postId);
        if (!result.Success || result.Value == null || result.Value.ParentId != null)
        {
            return result.ToErrorAs("Not a post");
        }

        return result;
    }

    public static async Task<Result> CreatePostAsync(this IPostsService postsService, Content post)
    {
        post.ParentId = null;
        return await postsService.CreateContentAsync(post);
    }

    public static async Task<Result<List<Content>>> GetAllPostsAsync(this IPostsService postsService, int page,
        int pageSize)
    {
        ContentFilter filter = content => content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<int>> GetAllPostsCountAsync(this IPostsService postsService)
    {
        ContentFilter filter = content => content.ParentId == null;
        return await postsService.GetContentCountAsync(filter);
    }

    public static async Task<Result<List<Content>>> GetPostsByUserAsync(this IPostsService postsService, ContentOrder order, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<List<Content>>> GetPostsByUserAsync(this IPostsService postsService, int userId, int page, int pageSize)
    {
        ContentFilter filter = content => content.UserId == userId && content.ParentId == null;
        ContentOrder order = contents => contents.OrderByDescending(p => p.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }
}