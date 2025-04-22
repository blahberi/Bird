using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public class LikesService : ILikesService
{
    private readonly IApiService apiService;
    private readonly IAuthService authService;

    public LikesService(IApiService apiService, IAuthService authService)
    {
        this.apiService = apiService;
        this.authService = authService;
    }

    public async Task<Result> LikePostAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result.FailureResult("Unauthorized");
        }
        return await apiService.PostAsync($"likes?contentId={postId}", new { });
    }

    public async Task<Result> UnlikePostAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result.FailureResult("Unauthorized");
        }
        return await apiService.DeleteAsync($"likes?contentId={postId}");
    }

    public async Task<Result<LikeCountResponse>> GetLikesCountByPostIdAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<LikeCountResponse>.FailureResult("Unauthorized");
        }
        return await apiService.GetAsync<LikeCountResponse>($"likes?contentId={postId}");
    }
}