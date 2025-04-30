using Backend.Core;
using Backend.Services;
using Shared;
using Shared.Extensions;

namespace Backend.Extensions;

public static class CommentsExtensions
{
    public static async Task<Result<None, Error>> AddCommentAsync(this IPostsService postsService, Content comment)
    {
        if (comment.ParentId == null)
        {
            return Error.CreateErr<None>("Parent post ID is required for comments");
        }

        return await postsService
            .GetContentById(comment.ParentId.Value)
            .AndThenAsync(async parent =>
            {
                if (parent.ParentId != null)
                {
                    return Error.CreateErr<None>("Cannot add a comment to another comment");
                }

                comment.ParentId = parent.Id;
                return await postsService.CreateContentAsync(comment);
            });
    }

    public static async Task<Result<IEnumerable<Content>, Error>> GetCommentsByPostIdAsync(this IPostsService postsService, int postId, int page, int pageSize)
    {
        ContentFilter filter = content => content.ParentId == postId;
        ContentOrder order = contents => contents.OrderByDescending(c => c.CreatedAt);
        return await postsService.GetContentsAsync(filter, order, page, pageSize);
    }

    public static async Task<Result<int, Error>> GetCommentsCountByPostIdAsync(this IPostsService postsService, int postId)
    {
        ContentFilter filter = content => content.ParentId == postId;
        return await postsService.GetContentCountAsync(filter);
    }
}