using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts.PostDetail;

public partial class PostDetailBase : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    protected IPostsService PostsService { get; set; } = default!;

    [Inject]
    protected ILikesService LikesService { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    protected PostResponse? post;
    protected bool loading = true;
    protected string? error;
    protected string? successMessage;
    protected bool showSuccessMessage = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadPost();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadPost();
    }

    protected async Task LoadPost()
    {
        loading = true;
        error = null;

        Result<PostResponse> result = await PostsService.GetPostByIdAsync(Id);

        if (!result.Success || result.Value == null)
        {
            error = result.Error ?? "Failed to load post";
        }
        else
        {
            post = result.Value;
        }

        loading = false;
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/posts");
    }

    protected async Task ScrollToComments()
    {
        await JSRuntime.InvokeVoidAsync("scrollToElement", "comments");
    }

    protected async Task ToggleLike()
    {
        if (post == null) return;

        Result result;

        if (post.IsLikedByCurrentUser)
        {
            result = await LikesService.UnlikePostAsync(post.Id);
            if (result.Success)
            {
                post.IsLikedByCurrentUser = false;
                post.LikesCount = Math.Max(0, post.LikesCount - 1);
                StateHasChanged();
            }
            else
            {
                error = result.Error ?? "Failed to unlike post";
            }
        }
        else
        {
            result = await LikesService.LikePostAsync(post.Id);
            if (result.Success)
            {
                post.IsLikedByCurrentUser = true;
                post.LikesCount++;
                StateHasChanged();
            }
            else
            {
                error = result.Error ?? "Failed to like post";
            }
        }
    }
}
