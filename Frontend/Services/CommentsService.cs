using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public class CommentsService : ICommentsService
{
    private readonly IApiService apiService;
    private readonly IAuthService authService;
    public CommentsService(IApiService apiService, IAuthService authService)
    {
        this.apiService = apiService;
        this.authService = authService;
    }

    public async Task<Result> AddCommentAsync(CommentCreation commentCreation)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result.FailureResult("Unauthorized");
        }
        return await apiService.PostAsync("comments", commentCreation);
    }

    public async Task<Result<(List<CommentResponse> Comments, int TotalCount)>> GetCommentsByPostIdAsync(int postId, int pageNumber, int pageSize)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<(List<CommentResponse> Comments, int TotalCount)>.FailureResult("Unauthorized");
        }
        Result<ListCommentResponse> result = await apiService.GetAsync<ListCommentResponse>($"comments/post/{postId}?pageNumber={pageNumber}&pageSize={pageSize}");
        if (!result.Success || result.Value == null)
        {
            if (result.Error == null && result.Errors != null)
            {
                return Result<(List<CommentResponse> Comments, int TotalCount)>.FailureResult(result.Errors);
            }
            if (result.Error != null)
            {
                return Result<(List<CommentResponse> Comments, int TotalCount)>.FailureResult(result.Error);
            }
            return Result<(List<CommentResponse> Comments, int TotalCount)>.FailureResult("An error occurred while fetching comments");
        }

        return Result<(List<CommentResponse> Comments, int TotalCount)>.SuccessResult((result.Value.Comments, result.Value.TotalCount));
    }
}