using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Services;

public interface IPostsService
{
    Task<Result<PostResponse>> GetPostByIdAsync(int id);
    Task<Result> CreatePostAsync(PostCreation postCreation);
    Task<Result<(List<PostResponse> Posts, int TotalCount)>> GetPostsAsync(int pageNumber, int pageSize);
    Task<Result<int>> GetPostsCountAsync();
}