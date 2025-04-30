using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Posts;

namespace Backend.Controllers;

[ApiController]
[Route("api/likes")]
public class LikesController : ControllerBase
{
    private readonly ILikesService likesService;

    public LikesController(ILikesService likesService)
    {
        this.likesService = likesService;
    }

    [HttpPost]
    public async Task<IActionResult> LikeContent(int contentId)
    {
        int userId = this.AuthorizeUser();

        await this.likesService.LikeContentAsync(userId, contentId);
        return this.Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> UnlikeContent(int contentId)
    {
        int userId = this.AuthorizeUser();

        await this.likesService.UnlikeContentAsync(userId, contentId);
        return this.Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetLikeCount(int contentId)
    {
        int count = await this.likesService.GetLikesCountByContentIdAsync(contentId);
        return this.Ok(new LikeCountResponse { Count = count });
    }
}