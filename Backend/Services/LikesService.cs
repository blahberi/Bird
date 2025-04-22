using Backend.Core;
using Backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Backend.Services;

internal class LikesService : ILikesService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IPostsService postsService;

    public LikesService(ApplicationDbContext dbContext, IPostsService postsService)
    {
        this.dbContext = dbContext;
        this.postsService = postsService;
    }

    public async Task<Result> LikeContentAsync(int contentId, int userId)
    {
        try
        {
            Result<Content> contentResult = await this.postsService.GetContentById(contentId);
            if (!contentResult.Success)
            {
                return Result.FailureResult(contentResult.Error);
            }

            Content? content = contentResult.Value;
            if (content == null)
            {
                return Result.FailureResult("Content not found");
            }

            bool alreadyLiked = await this.dbContext.Likes
                .AnyAsync(l => l.ContentId == contentId && l.UserId == userId);

            if (alreadyLiked)
            {
                return Result.FailureResult("User has already liked this content");
            }

            await this.dbContext.Likes.AddAsync(new Like
            {
                ContentId = contentId,
                UserId = userId
            });
            await this.dbContext.SaveChangesAsync();
            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error liking content: {ex}");
            return Result.FailureResult("Failed to like content");
        }
    }

    public async Task<Result> UnlikeContentAsync(int contentId, int userId)
    {
        try
        {
            Like? like = await this.dbContext.Likes
                .FirstOrDefaultAsync(l => l.ContentId == contentId && l.UserId == userId);

            if (like == null)
            {
                return Result.FailureResult("User has not liked this content");
            }

            this.dbContext.Likes.Remove(like);
            await this.dbContext.SaveChangesAsync();
            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unliking content: {ex}");
            return Result.FailureResult("Failed to unlike content");
        }
    }

    public async Task<Result<bool>> HasUserLikedContentAsync(int contentId, int userId)
    {
        try
        {
            bool hasLiked = await this.dbContext.Likes
                .AnyAsync(l => l.ContentId == contentId && l.UserId == userId);

            return Result<bool>.SuccessResult(hasLiked);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking if user liked content: {ex}");
            return Result<bool>.FailureResult("Failed to check if user liked content");
        }
    }

    public async Task<Result<List<Like>>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize)
    {
        try
        {
            IQueryable<Like> query = this.dbContext.Likes
                .Include(l => l.User)
                .Include(l => l.Content)
                .Where(filter);
            query = order(query);
            List<Like> likes = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<Like>>.SuccessResult(likes);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving likes: {e}");
            return Result<List<Like>>.FailureResult("An error occurred while retrieving likes");
        }
    }

    public async Task<Result<int>> GetLikesCountAsync(LikeFilter filter)
    {
        try
        {
            int count = await this.dbContext.Likes
                .Where(filter)
                .CountAsync();

            return Result<int>.SuccessResult(count);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting likes count: {e}");
            return Result<int>.FailureResult("An error occurred while retrieving likes count");
        }
    }
}