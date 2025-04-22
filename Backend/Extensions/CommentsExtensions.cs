using Backend.Core;
using Backend.Services;
using Shared;

namespace Backend.Extensions;

public static class CommentsExtensions
{
    public static async Task<Result> AddCommentAsync(this IPostsService postsService, Content comment)
    {
        if (comment.ParentId == null)
        {
            return Result.FailureResult("Parent post ID is required for comments");
        }

        // Verify the parent post exists
        Result<Content> parentResult = await postsService.GetContentById(comment.ParentId.Value);
        if (!parentResult.Success || parentResult.Value == null)
        {
            return Result.FailureResult("Parent post not found");
        }

        // Verify the parent is actually a post (not another comment)
        if (parentResult.Value.ParentId != null)
        {
            return Result.FailureResult("Cannot add a comment to another comment");
        }

        return await postsService.CreateContentAsync(comment);
    }

    public static async Task<Result<List<Content>>> GetCommentsByPostIdAsync(this IPostsService postsService, int postId, int page, int pageSize)
    {
        ContentFilter filter = content => content.ParentId == postId;
        ContentOrder order = contents => contents.OrderByDescending(c => c.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<int>> GetCommentsCountByPostIdAsync(this IPostsService postsService, int postId)
    {
        ContentFilter filter = content => content.ParentId == postId;
        return await postsService.GetContentCountAsync(filter);
    }
}