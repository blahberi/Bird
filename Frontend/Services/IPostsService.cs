using Shared;
using Shared.DTOs.Posts;
using Frontend.Core;

namespace Frontend.Services;

public interface IPostsService
{
    Task<Result<PostResponse, Error>> GetPostByIdAsync(int id);
    Task<Result<None, Error>> CreatePostAsync(PostCreation postCreation);
    Task<Result<ListPostResponse, Error>> GetPostsAsync(int pageNumber, int pageSize);
    Task<Result<int, Error>> GetPostsCountAsync();
}