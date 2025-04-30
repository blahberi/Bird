using Backend.Core;
using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs.Posts;
using Shared.Extensions;

namespace Backend.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IPostsService postsService;
    private readonly ILikesService likesService;

    public PostsController(IPostsService postsService, ILikesService likesService)
    {
        this.postsService = postsService;
        this.likesService = likesService;
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostById(int postId)
    {
        return await this
            .AuthorizeUser()
            .AndThenAsync(async userId => await this.GetPostById(userId, postId))
            .ToActionResultAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostCreation postCreation)
    {
        return await this
            .AuthorizeUser()
            .AndThenAsync(async userId =>
            {
                Content post = new Content
                {
                    UserId = userId,
                    Title = postCreation.Title,
                    ContentText = postCreation.Content,
                    ParentId = null
                };
                return await this.postsService.CreatePostAsync(post)
                    .MapAsync<None, object, Error>(async _ => new object());
            })
            .ToActionResultAsync();
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return await this
            .AuthorizeUser()
            .AndThenAsync(async _ =>
            {
                return await this.postsService.GetAllPostsAsync(pageNumber, pageSize);
            })
            .AndThenAsync(async posts =>
            {
                return posts.Select(post => new PostResponse
                {
                    Id = post.Id,
                    Title = post.Title ?? string.Empty,
                    Content = post.ContentText,
                    UserId = post.UserId,
                    UserName = post.User.Username,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    LikesCount = post.Likes.Count
                }).ToOkResult();
            })
            .AndThenAsync(async posts =>
            {
                return await this.postsService.GetAllPostsCountAsync()
                    .AndThenAsync(async count => new ListPostResponse
                    {
                        Posts = posts,
                        TotalCount = count
                    }.ToOkResult());
            })
            .ToActionResultAsync();
    }
    private async Task<Result<PostResponse, Error>> GetPostById(int userId, int postId)
    {
        return await this.postsService.GetPostByIdAsync(postId)
            .AndThenAsync(async post =>
            {
                Result<bool, Error> likedResult = await this.likesService.HasUserLikedContentAsync(postId, userId);
                return likedResult.Map(isLiked => (post, isLiked));
            })
            .MapAsync<(Content Post, bool isLiked), PostResponse, Error>(async post => new PostResponse
            {
                Id = post.Post.Id,
                Title = post.Post.Title ?? string.Empty,
                Content = post.Post.ContentText,
                UserId = post.Post.UserId,
                UserName = post.Post.User.Username,
                CreatedAt = post.Post.CreatedAt,
                UpdatedAt = post.Post.UpdatedAt,
                LikesCount = post.Post.Likes.Count,
            });
    }
}
