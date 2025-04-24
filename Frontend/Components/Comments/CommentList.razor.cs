using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Components.Comments;

public partial class CommentListBase : ComponentBase
{
    [Parameter] public int PostId { get; set; }
    [Parameter] public EventCallback OnCommentsChanged { get; set; }

    [Inject] protected ICommentsService CommentsService { get; set; } = default!;

    private const int PageSize = 10;

    protected List<CommentModel> comments = new();
    protected CommentCreation newComment = new();
    protected string? errorMessage;
    protected int currentPage;
    protected int totalCount;

    protected bool isLoading = true;
    protected bool isSubmitting;
    protected bool isLoadingMore;

    protected bool HasMoreComments => this.comments.Count < this.totalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadComments();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (PostId > 0)
        {
            await LoadComments();
        }
    }

    protected async Task LoadComments()
    {
        this.isLoading = true;
        this.currentPage = 0;
        this.comments.Clear();
        await FetchComments();
        this.isLoading = false;
    }

    protected async Task LoadNextPage()
    {
        this.currentPage++;
        this.isLoadingMore = true;
        await FetchComments();
        this.isLoadingMore = false;
    }

    protected async Task FetchComments()
    {
        this.errorMessage = null;

        Result<(List<CommentResponse> Comments, int TotalCount)> result = await CommentsService.GetCommentsByPostIdAsync(PostId, this.currentPage, PageSize);

        if (result.Success)
        {
            List<CommentResponse> newComments = result.Value.Comments;
            this.totalCount = result.Value.TotalCount;

            this.comments.AddRange(newComments.Select(c => new CommentModel
            {
                Id = c.Id,
                Content = c.Content,
                UserName = c.UserName,
                CreatedAt = c.CreatedAt,
                IsLikedByCurrentUser = c.IsLikedByCurrentUser,
                LikesCount = c.LikesCount
            }));
        }
        else
        {
            this.errorMessage = result.Error ?? "Failed to load comments";
        }
    }

    protected async Task HandleSubmit()
    {
        this.isSubmitting = true;
        this.errorMessage = null;

        this.newComment.ContentId = PostId;
        this.newComment.PostId = PostId;
        Result result = await CommentsService.AddCommentAsync(this.newComment);

        if (result.Success)
        {
            this.newComment = new CommentCreation();
            await LoadComments();
            await OnCommentsChanged.InvokeAsync();
        }
        else
        {
            this.errorMessage = result.Error ?? "Failed to add comment";
        }

        this.isSubmitting = false;
    }
}
