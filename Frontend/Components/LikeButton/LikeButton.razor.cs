using Microsoft.AspNetCore.Components;
using Frontend.Services;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Components.LikeButton;

public partial class LikeButton : ComponentBase
{
    [Parameter]
    public int PostId { get; set; }

    [Parameter]
    public bool InitialIsLiked { get; set; }

    [Parameter]
    public bool ShowCount { get; set; } = true;

    [Parameter]
    public bool Small { get; set; } = false;

    [Inject]
    private ILikesService LikesService { get; set; } = default!;

    private bool isLiked;
    private int likesCount;
    private bool isLoading;

    private bool IsLiked
    {
        get => this.isLiked;
        set
        {
            if (this.isLiked != value)
            {
                this.isLiked = value;
            }
        }
    }

    private int LikesCount
    {
        get => this.likesCount;
        set
        {
            if (this.likesCount != value)
            {
                this.likesCount = value;
                StateHasChanged();
            }
        }
    }

    private bool IsLoading
    {
        get => this.isLoading;
        set
        {
            if (this.isLoading != value)
            {
                this.isLoading = value;
                StateHasChanged();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        this.isLiked = InitialIsLiked;
        await LoadLikesCount();
    }

    private async Task LoadLikesCount()
    {
        try
        {
            Result<LikeCountResponse> result = await LikesService.GetLikesCountByPostIdAsync(PostId);
            if (!result.Success || result.Value == null)
            {
                LikesCount = 0;
                return;
            }

            LikesCount = result.Value.Count;
        }
        catch
        {
            LikesCount = 0;
        }
    }

    private async Task ToggleLike()
    {
        if (IsLoading) return;

        IsLoading = true;

        bool originalIsLiked = IsLiked;
        int originalLikesCount = LikesCount;

        IsLiked = !originalIsLiked;
        LikesCount = IsLiked ? originalLikesCount + 1 : originalLikesCount - 1;

        try
        {
            Result result = IsLiked
                ? await LikesService.LikePostAsync(PostId)
                : await LikesService.UnlikePostAsync(PostId);

            if (!result.Success)
            {
                IsLiked = originalIsLiked;
                LikesCount = originalLikesCount;
            }
        }
        catch
        {
            IsLiked = originalIsLiked;
            LikesCount = originalLikesCount;
        }
        finally
        {
            IsLoading = false;
        }
    }
}