using Backend.Core;
using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs.Posts;

namespace Backend.Controllers;

[ApiController]
[Route("api/likes")]
public class LikesController : AuthControllerBase
{
    private readonly ILikesService likesService;

    public LikesController(ILikesService likesService)
    {
        this.likesService = likesService;
    }

    [HttpPost]
    public async Task<IActionResult> LikeContent(int contentId)
    {
        int? userId = this.AuthorizeUser();
        if (userId == null)
        {
            return this.UnauthorizedUser();
        }

        Like like = new Like
        {
            UserId = (int)userId,
            ContentId = contentId
        };

        Result result = await this.likesService.LikeContentAsync(like.ContentId, like.UserId);

        if (result.Success)
        {
            return this.Ok(new { id = like.Id });
        }

        return this.BadRequest(new { error = result.Error });
    }

    [HttpDelete]
    public async Task<IActionResult> UnlikeContent(int contentId)
    {
        int? userId = this.AuthorizeUser();
        if (userId == null)
        {
            return this.UnauthorizedUser();
        }

        Result result = await this.likesService.UnlikeContentAsync(contentId, (int)userId);

        if (result.Success)
        {
            return this.Ok();
        }

        return this.BadRequest(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetLikeCount(int contentId)
    {
        Result<int> result = await this.likesService.GetLikesCountByContentIdAsync(contentId);

        if (!result.Success || result.Value == null)
        {
            return this.BadRequest(new { error = result.Error });
        }

        return this.Ok(new LikeCountResponse { Count = result.Value });
    }
}