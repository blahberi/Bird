using Backend.Core;
using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs.Posts;
using Shared.Extensions;

namespace Backend.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IPostsService postsService;
    private readonly ILikesService likesService;

    public CommentsController(IPostsService postsService, ILikesService likesService)
    {
        this.postsService = postsService;
        this.likesService = likesService;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentCreation commentCreation)
    {
        int userId = this.AuthorizeUser();
        Content content = await this.postsService.GetContentById(commentCreation.ContentId);

        await this.postsService.AddCommentAsync(new Content
        {
            UserId = userId,
            ParentId = content.Id,
            PostId = content.PostId,
            ContentText = commentCreation.Content,
            Title = null
        });

        return this.Ok();
    }

    [HttpGet("post/{contentId}")]
    public async Task<IActionResult> GetPostComments([FromQuery] int pageNumber, [FromQuery] int pageSize, int contentId)
    {
        int userId = this.AuthorizeUser();

        IEnumerable<Content> comments = await this.postsService.GetCommentsByPostIdAsync(contentId, pageNumber, pageSize);
        IEnumerable<CommentResponse> commentResponses = comments.Select(comment => new CommentResponse
        {
            Id = comment.Id,
            Content = comment.ContentText,
            UserId = comment.UserId,
            UserName = comment.User.Username,
            ContentId = comment.ParentId ?? 0,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
        });

        int totalCount = await this.postsService.GetCommentsCountByPostIdAsync(contentId);

        return this.Ok(new ListCommentResponse
        {
            Comments = commentResponses,
            TotalCount = totalCount
        });
    }
}