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
public class PostsController : AuthControllerBase
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
        int? currentUserId = this.AuthorizeUser();
        if (currentUserId == null)
        {
            return this.UnauthorizedUser();
        }

        Result<Content> result = await this.postsService.GetPostByIdAsync(postId);
        if (!result.Success || result.Value == null)
        {
            return this.NotFound(result.Error);
        }

        Content post = result.Value;
        bool isLikedByCurrentUser = false;

        Result<bool> likedResult = await this.likesService.HasUserLikedContentAsync(postId, currentUserId.Value);
        if (likedResult.Success)
        {
            isLikedByCurrentUser = likedResult.Value;
        }

        return this.Ok(new PostResponse
        {
            Id = post.Id,
            Title = post.Title ?? string.Empty,
            Content = post.ContentText,
            UserId = post.UserId,
            UserName = post.User.Username,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            LikesCount = post.Likes.Count,
            IsLikedByCurrentUser = isLikedByCurrentUser
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostCreation postCreation)
    {
        int? userId = this.AuthorizeUser();
        if (userId == null)
        {
            return this.UnauthorizedUser();
        }
        Content post = new Content
        {
            UserId = (int)userId,
            Title = postCreation.Title,
            ContentText = postCreation.Content,
            ParentId = null
        };

        Result result = await this.postsService.CreatePostAsync(post);
        if (result.Success)
        {
            return this.Ok(new { id = post.Id });
        }
        return this.BadRequest(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        int? currentUserId = this.AuthorizeUser();
        if (currentUserId == null)
        {
            return this.UnauthorizedUser();
        }

        Result<List<Content>> result = await this.postsService.GetAllPostsAsync(pageNumber, pageSize);
        if (!result.Success || result.Value == null)
        {
            return this.BadRequest(new { error = result.Error });
        }
        List<Content> posts = result.Value;

        Result<int> countResult = await this.postsService.GetAllPostsCountAsync();
        int totalCount = countResult.Success ? countResult.Value : 0;

        List<PostResponse> postsResponse = new List<PostResponse>();

        foreach (var post in posts)
        {
            bool isLikedByCurrentUser = false;

            if (currentUserId.HasValue)
            {
                Result<bool> likedResult = await this.likesService.HasUserLikedContentAsync(post.Id, currentUserId.Value);
                if (likedResult.Success)
                {
                    isLikedByCurrentUser = likedResult.Value;
                }
            }

            postsResponse.Add(new PostResponse
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                Content = post.ContentText,
                UserId = post.UserId,
                UserName = post.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikesCount = post.Likes.Count,
                IsLikedByCurrentUser = isLikedByCurrentUser
            });
        }

        return this.Ok(new ListPostResponse
        {
            Posts = postsResponse,
            TotalCount = totalCount
        });
    }
}