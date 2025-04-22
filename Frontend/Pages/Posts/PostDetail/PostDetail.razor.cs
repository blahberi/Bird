using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts.PostDetail;

public partial class PostDetail : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    private IPostsService PostsService { get; set; } = default!;

    [Inject]
    private ILikesService LikesService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private PostResponse? post;
    private bool loading = true;
    private string? error;
    private string? successMessage;
    private bool showSuccessMessage = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadPost();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadPost();
    }

    private async Task LoadPost()
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

    private void GoBack()
    {
        NavigationManager.NavigateTo("/posts");
    }

    private async Task ScrollToComments()
    {
        await JSRuntime.InvokeVoidAsync("scrollToElement", "comments");
    }

    private async Task ToggleLike()
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