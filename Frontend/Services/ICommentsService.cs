using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public interface ICommentsService
{
    Task<Result<None, string>> AddCommentAsync(CommentCreation commentCreation);
    Task<Result<List<ListCommentResponse>, string>> GetCommentsByPostIdAsync(int postId, int pageNumber, int pageSize);
}