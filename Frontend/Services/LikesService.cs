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

    public async Task<Result<None, string>> LikePostAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<None, string>.CreateErr("Unauthorized");
        }
        return await apiService.PostAsync($"likes?contentId={postId}", new { });
    }

    public async Task<Result<None, string>> UnlikePostAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<None, string>.CreateErr("Unauthorized");
        }
        return await apiService.DeleteAsync($"likes?contentId={postId}");
    }

    public async Task<Result<LikeCountResponse, string>> GetLikesCountByPostIdAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<LikeCountResponse, string>.CreateErr("Unauthorized");
        }
        return await apiService.GetAsync<LikeCountResponse>($"likes?contentId={postId}");
    }

    public async Task<Result<bool, string>> HasUserLikedPostAsync(int postId)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<bool, string>.CreateErr("Unauthorized");
        }
        return await apiService.GetAsync<bool>($"likes?contentId={postId}");
    }
}