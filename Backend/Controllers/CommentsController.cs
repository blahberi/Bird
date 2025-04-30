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
        return await this
            .AuthorizeUser()
            .AndThenAsync(async userId =>
            {
                Content comment = new Content
                {
                    UserId = userId,
                    ParentId = commentCreation.ContentId,
                    PostId = commentCreation.PostId,
                    ContentText = commentCreation.Content,
                    Title = null
                };

                return await this.postsService.AddCommentAsync(comment)
                    .MapAsync<None, object, Error>(async _ => new { id = comment.Id });
            })
            .ToActionResultAsync();
    }

    [HttpGet("post/{contentId}")]
    public async Task<IActionResult> GetPostComments([FromQuery] int pageNumber, [FromQuery] int pageSize, int contentId)
    {
        return await this
            .AuthorizeUser()
            .AndThenAsync(async _ =>
            {
                return await this.postsService.GetCommentsByPostIdAsync(contentId, pageNumber, pageSize);
            })
            .AndThenAsync(async comments =>
            {
                return comments.Select(comment => new CommentResponse
                {
                    Id = comment.Id,
                    Content = comment.ContentText,
                    UserId = comment.UserId,
                    UserName = comment.User.Username,
                    ContentId = comment.ParentId ?? 0,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    LikesCount = comment.Likes.Count
                }).ToOkResult();
            })
            .AndThenAsync(async responses =>
            {
                return await this.postsService.GetCommentsCountByPostIdAsync(contentId)
                    .AndThenAsync(async count =>
                    {
                        return new ListCommentResponse
                        {
                            Comments = responses,
                            TotalCount = count
                        }.ToOkResult();
                    });
            })
            .ToActionResultAsync();
    }
}