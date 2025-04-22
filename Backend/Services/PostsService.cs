using Backend.DataAccess;
using Backend.Core;
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

    public async Task<Result<Content>> GetContentById(int id)
    {
        try
        {
            Content? content = await this.dbContext.Contents
                .Include(c => c.User)
                .Include(c => c.Likes)
                    .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (content == null)
            {
                return Result<Content>.FailureResult("Content not found");
            }

            return content.ToResult();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting content: {e}");
            return Result<Content>.FailureResult("An error occurred while retrieving the content");
        }
    }

    public async Task<Result> CreateContentAsync(Content content)
    {
        try
        {
            await this.dbContext.Contents.AddAsync(content);
            await this.dbContext.SaveChangesAsync();
            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating content: {ex}");
            return Result.FailureResult("An error occurred while creating the content");
        }
    }

    public async Task<Result<List<Content>>> GetContentsAsync(ContentFilter filter, ContentOrder order, int page, int pageSize)
    {
        try
        {
            IQueryable<Content> query = this.dbContext.Contents
                .Include(c => c.User)
                .Include(c => c.Likes)
                    .ThenInclude(l => l.User)
                .Where(filter);
            query = order(query);
            List<Content> contents = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<List<Content>>.SuccessResult(contents);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving contents: {e}");
            return Result<List<Content>>.FailureResult("An error occurred while retrieving contents");
        }
    }

    public async Task<Result<int>> GetContentCountAsync(ContentFilter filter)
    {
        try
        {
            int count = await this.dbContext.Contents
                .Where(filter)
                .CountAsync();

            return Result<int>.SuccessResult(count);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting content count: {e}");
            return Result<int>.FailureResult("An error occurred while retrieving content count");
        }
    }
}
