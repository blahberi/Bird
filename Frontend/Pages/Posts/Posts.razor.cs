using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts;

public partial class Posts : ComponentBase
{
    [Inject]
    private IPostsService PostsService { get; set; }

    [Inject]
    private ILikesService LikesService { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    private List<PostResponse> posts;
    private int currentPage = 0;
    private int pageSize = 10;
    private int totalPages = 1;

    protected override async Task OnInitializedAsync()
    {
        await LoadPosts();
    }

    private async Task LoadPosts()
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

    private async Task PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            await LoadPosts();
        }
    }

    private async Task NextPage()
    {
        currentPage++;
        await LoadPosts();
    }

    private async Task GoToPage(int page)
    {
        currentPage = page;
        await LoadPosts();
    }

    private void CreateNewPost()
    {
        NavigationManager.NavigateTo("/posts/create");
    }

    private void ViewPost(int id)
    {
        NavigationManager.NavigateTo($"/posts/{id}");
    }
}