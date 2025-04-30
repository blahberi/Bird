using Backend.DataAccess;
using Backend.Core;
using Backend.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Extensions;

namespace Backend.Services;

internal class PostsService : IPostsService
{
    private readonly ApplicationDbContext dbContext;

    public PostsService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Result<Content, Error>> GetContentById(int id)
    {
        return await this.dbContext.Contents
            .Include(c => c.User)
            .Include(c => c.Likes)
            .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(c => c.Id == id)
            .ToResultAsync("Content not found");
    }

    public async Task<Result<None, Error>> CreateContentAsync(Content content)
    {
        await this.dbContext.Contents.AddAsync(content);
        await this.dbContext.SaveChangesAsync();
        return None.Value.ToOkResult();
    }

    public async Task<Result<IEnumerable<Content>, Error>> GetContentsAsync(ContentFilter filter, ContentOrder order, int page, int pageSize)
    {
        IQueryable<Content> query = this.dbContext.Contents
            .Include(c => c.User)
            .Include(c => c.Likes)
            .ThenInclude(l => l.User)
            .Where(filter);
        query = order(query);
        IEnumerable<Content> contents = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return contents.ToEnumerableResult("No contents found");
    }

    public async Task<Result<int, Error>> GetContentCountAsync(ContentFilter filter)
    {
        return await this.dbContext.Contents
            .Where(filter)
            .CountAsync()
            .ToOkResultAsync();
    }
}