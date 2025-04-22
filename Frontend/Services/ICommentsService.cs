using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public interface ICommentsService
{
    Task<Result> AddCommentAsync(CommentCreation commentCreation);
    Task<Result<(List<CommentResponse> Comments, int TotalCount)>> GetCommentsByPostIdAsync(int postId, int pageNumber, int pageSize);
}