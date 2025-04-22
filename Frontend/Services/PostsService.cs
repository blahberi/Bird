using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public class PostsService : IPostsService
{
    private readonly IApiService apiService;
    private readonly IAuthService authService;
    public PostsService(IApiService apiService, IAuthService authService)
    {
        this.apiService = apiService;
        this.authService = authService;
    }

    public async Task<Result<PostResponse>> GetPostByIdAsync(int id)
    {
        return await apiService.GetAsync<PostResponse>($"posts/{id}");
    }

    public async Task<Result> CreatePostAsync(PostCreation postCreation)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result.FailureResult("Unauthorized");
        }
        return await apiService.PostAsync("posts", postCreation);
    }

    public async Task<Result<(List<PostResponse> Posts, int TotalCount)>> GetPostsAsync(int pageNumber, int pageSize)
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<(List<PostResponse> Posts, int TotalCount)>.FailureResult("Unauthorized");
        }
        string queryString = $"posts?pageNumber={pageNumber}&pageSize={pageSize}";
        Result<ListPostResponse> result = await apiService.GetAsync<ListPostResponse>(queryString);

        if (!result.Success || result.Value == null)
        {
            if (result.Error == null && result.Errors != null)
            {
                return Result<(List<PostResponse> Posts, int TotalCount)>.FailureResult(result.Errors);
            }
            if (result.Error != null)
            {
                return Result<(List<PostResponse> Posts, int TotalCount)>.FailureResult(result.Error);
            }
            return Result<(List<PostResponse> Posts, int TotalCount)>.FailureResult("An error occurred while fetching posts");
        }

        return Result<(List<PostResponse> Posts, int TotalCount)>.SuccessResult((result.Value.Posts, result.Value.TotalCount));
    }

    public async Task<Result<int>> GetPostsCountAsync()
    {
        if (!await authService.IsAuthorizedAsync)
        {
            return Result<int>.FailureResult("Unauthorized");
        }
        string queryString = $"posts?pageNumber=0&pageSize=1";
        Result<ListPostResponse> result = await apiService.GetAsync<ListPostResponse>(queryString);

        if (!result.Success || result.Value == null)
        {
            if (result.Error == null && result.Errors != null)
            {
                return Result<int>.FailureResult(result.Errors);
            }
            if (result.Error != null)
            {
                return Result<int>.FailureResult(result.Error);
            }
            return Result<int>.FailureResult("An error occurred while fetching post count");
        }

        return Result<int>.SuccessResult(result.Value.TotalCount);
    }
}