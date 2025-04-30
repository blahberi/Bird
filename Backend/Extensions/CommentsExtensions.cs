using Backend.Core;
using Backend.Services;
using Shared;
using Shared.Extensions;

namespace Backend.Extensions;

public static class CommentsExtensions
{
    public static async Task AddCommentAsync(this IPostsService postsService, Content comment)
    {
        if (comment.ParentId == null)
        {
            throw new Exception("Parent post ID is required for comments");
        }

        Content parent = await postsService.GetContentById(comment.ParentId.Value);
        if (parent.ParentId != null)
        {
            throw new Exception("Cannot add a comment to another comment");
        }

        comment.ParentId = parent.Id;
        await postsService.CreateContentAsync(comment);
    }

    public static async Task<IEnumerable<Content>> GetCommentsByPostIdAsync(this IPostsService postsService, int postId, int page, int pageSize)
    {
        ContentFilter filter = content => content.ParentId == postId;
        ContentOrder order = contents => contents.OrderByDescending(c => c.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<int> GetCommentsCountByPostIdAsync(this IPostsService postsService, int postId)
    {
        ContentFilter filter = content => content.ParentId == postId;
        return await postsService.GetContentCountAsync(filter);
    }
}