using Backend.Core;
using Backend.DataAccess;
using Backend.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Extensions;

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

    public async Task<Result<None, Error>> LikeContentAsync(int contentId, int userId)
    {
        return await postsService.GetContentById(contentId)
            .AndThenAsync(this.LikeContentAsync);
    }

    public async Task<Result<None, Error>> UnlikeContentAsync(int contentId, int userId)
    {
        return await this.dbContext.Likes
            .FirstOrDefaultAsync(l => l.ContentId == contentId && l.UserId == userId)
            .ToResultAsync("User has not liked this content")
            .AndThenAsync(async like => {
                this.dbContext.Likes.Remove(like);
                await this.dbContext.SaveChangesAsync();
                return None.Value.ToOkResult();
            });
    }

    public async Task<Result<bool, Error>> HasUserLikedContentAsync(int contentId, int userId)
    {
        return await this.dbContext.Likes
            .AnyAsync(l => l.ContentId == contentId && l.UserId == userId)
            .ToOkResultAsync();
    }
    
    public async Task<Result<IEnumerable<bool>, Error>> HasUserLikedContentsAsync(int userId, IEnumerable<int> contentIds)
    {
        IEnumerable<int> contents = await this.dbContext.Likes
            .Where(l => l.UserId == userId && contentIds.Contains(l.ContentId))
            .Select(l => l.ContentId)
            .ToListAsync();
        
        IEnumerable<bool> result = contentIds
            .Select(contentId => contents.Contains(contentId))
            .ToList();
        
        return result.ToOkResult();
    }

    public async Task<Result<IEnumerable<Like>, Error>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize)
    {
        IQueryable<Like> query = this.dbContext.Likes
            .Include(l => l.User)
            .Include(l => l.Content)
            .Where(filter);
        query = order(query);

        IEnumerable<Like> likes = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return likes.ToOkResult();
    }

    public async Task<Result<int, Error>> GetLikesCountAsync(LikeFilter filter)
    {
        return await this.dbContext.Likes
            .Where(filter)
            .CountAsync()
            .ToOkResultAsync();
    }

    private async Task<Result<None, Error>> LikeContentAsync(Content content)
    {
        return await this.dbContext.Likes
            .AnyAsync(l => l.ContentId == content.Id && l.UserId == content.UserId)
            .ErrIfAsync(alreadyLiked => alreadyLiked, "User has already liked this content")
            .AndThenAsync(async _ =>
            {
                await this.dbContext.Likes
                    .AddAsync(new Like
                    {
                        ContentId = content.Id,
                        UserId = content.UserId
                    });
                await this.dbContext.SaveChangesAsync();
                return None.Value.ToOkResult();
            });
    }
}