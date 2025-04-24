using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts;

public partial class PostsBase : ComponentBase
{
    [Inject]
    protected IPostsService PostsService { get; set; } = default!;

    [Inject]
    protected ILikesService LikesService { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    protected List<PostResponse> posts;
    protected int currentPage = 0;
    protected int pageSize = 10;
    protected int totalPages = 1;

    protected override async Task OnInitializedAsync()
    {
        await LoadPosts();
    }

    protected async Task LoadPosts()
    {
        Result<(List<PostResponse> Posts, int TotalCount)> result = await PostsService.GetPostsAsync(currentPage, pageSize);
        if (!result.Success || result.Value.Posts == null)
        {
            Console.WriteLine(result.Error);
            return;
        }

        posts = result.Value.Posts;
        int totalCount = result.Value.TotalCount;

        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
    }

    protected async Task PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            await LoadPosts();
        }
    }

    protected async Task NextPage()
    {
        currentPage++;
        await LoadPosts();
    }

    protected async Task GoToPage(int page)
    {
        currentPage = page;
        await LoadPosts();
    }

    protected void CreateNewPost()
    {
        NavigationManager.NavigateTo("/posts/create");
    }

    protected void ViewPost(int id)
    {
        NavigationManager.NavigateTo($"/posts/{id}");
    }
}
