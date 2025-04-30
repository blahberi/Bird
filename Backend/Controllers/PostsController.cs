using System.Reflection.Metadata.Ecma335;
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
        int userId = this.AuthorizeUser();
        await this.postsService.GetContentById(postId);
        return this.Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostCreation postCreation)
    {
        int userId = this.AuthorizeUser();
        Content post = new Content
        {
            UserId = userId,
            Title = postCreation.Title,
            ContentText = postCreation.Content,
            ParentId = null
        };
        await this.postsService.CreatePostAsync(post);
        return await this.GetPostById(post.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        int userId = this.AuthorizeUser();
        IEnumerable<Content> posts = await this.postsService.GetAllPostsAsync(pageNumber, pageSize);
        IEnumerable<PostResponse> postResponses = posts.Select(post => new PostResponse
        {
            Id = post.Id,
            Title = post.Title ?? string.Empty,
            Content = post.ContentText,
            UserId = post.UserId,
            UserName = post.User.Username,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            LikesCount = post.Likes.Count
        });
        int totalCount = await this.postsService.GetAllPostsCountAsync();
        return this.Ok(new ListPostResponse
        {
            Posts = postResponses,
            TotalCount = totalCount
        });
    }
}