using Backend.Core;
using Backend.DataAccess;
using Microsoft.EntityFrameworkCore;

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

    public async Task LikeContentAsync(int contentId, int userId)
    {
        if (await this.HasUserLikedContentAsync(contentId, userId))
        {
            throw new Exception("User has already liked this content");
        }

        Content _ = await this.postsService.GetContentById(contentId); // Check if the content exists

        Like like = new Like
        {
            ContentId = contentId,
            UserId = userId,
        };

        await this.dbContext.Likes.AddAsync(like);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task UnlikeContentAsync(int contentId, int userId)
    {
        Like? like = await this.dbContext.Likes
            .FirstOrDefaultAsync(l => l.ContentId == contentId && l.UserId == userId);
        
        if (like == null)
        {
            throw new Exception("User has not liked this content");
        }

        this.dbContext.Likes.Remove(like);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasUserLikedContentAsync(int contentId, int userId)
    {
        return await this.dbContext.Likes
            .AnyAsync(l => l.ContentId == contentId && l.UserId == userId);
    }

    public async Task<IEnumerable<Like>> GetLikesAsync(LikeFilter filter, LikeOrder order, int page, int pageSize)
    {
        IQueryable<Like> query = this.dbContext.Likes
            .Include(l => l.User)
            .Include(l => l.Content)
            .Where(filter);
        
        query = order(query);

        IEnumerable<Like> likes = query
            .Skip(page * pageSize)
            .Take(pageSize);
            
        return likes;
    }

    public async Task<int> GetLikesCountAsync(LikeFilter filter)
    {
        return await this.dbContext.Likes
            .Where(filter)
            .CountAsync();
    }
}