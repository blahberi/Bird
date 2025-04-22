using Backend.Core;
using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs.Posts;

namespace Backend.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : AuthControllerBase
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
        int? userId = this.AuthorizeUser();
        if (userId == null)
        {
            return this.UnauthorizedUser();
        }

        Content comment = new Content
        {
            UserId = (int)userId,
            ParentId = commentCreation.ContentId,
            PostId = commentCreation.PostId,
            ContentText = commentCreation.Content,
            Title = null
        };

        Result result = await this.postsService.AddCommentAsync(comment);

        if (result.Success)
        {
            return this.Ok(new { id = comment.Id });
        }

        return this.BadRequest(new { error = result.Error });
    }

    [HttpGet("post/{contentId}")]
    public async Task<IActionResult> GetPostComments([FromQuery] int pageNumber, [FromQuery] int pageSize, int contentId)
    {
        int? userId = this.AuthorizeUser();
        if (userId == null)
        {
            return this.UnauthorizedUser();
        }

        Result<List<Content>> result = await this.postsService.GetCommentsByPostIdAsync(contentId, pageNumber, pageSize);

        if (!result.Success || result.Value == null)
        {
            return this.BadRequest(new { error = result.Error });
        }

        List<Content> comments = result.Value;

        Result<int> countResult = await this.postsService.GetCommentsCountByPostIdAsync(contentId);
        int totalCount = countResult.Success ? countResult.Value : 0;

        List<CommentResponse> response = new List<CommentResponse>();

        foreach (var comment in comments)
        {
            bool isLikedByCurrentUser = false;

            // Check if the current user has liked this comment
            Result<bool> likedResult = await this.likesService.HasUserLikedContentAsync(comment.Id, userId.Value);
            if (likedResult.Success)
            {
                isLikedByCurrentUser = likedResult.Value;
            }

            response.Add(new CommentResponse
            {
                Id = comment.Id,
                Content = comment.ContentText,
                UserId = comment.UserId,
                UserName = comment.User.Username,
                ContentId = comment.ParentId ?? 0,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                IsLikedByCurrentUser = isLikedByCurrentUser,
                LikesCount = comment.Likes.Count
            });
        }

        return this.Ok(new ListCommentResponse
        {
            Comments = response,
            TotalCount = totalCount
        });
    }
}