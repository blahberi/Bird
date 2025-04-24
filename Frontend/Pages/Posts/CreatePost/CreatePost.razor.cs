using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts.CreatePost;

public partial class CreatePostBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IPostsService PostsService { get; set; } = default!;

    protected PostCreation postModel = new();
    protected bool isSubmitting = false;
    protected string? errorMessage;

    protected async Task HandleSubmit()
    {
        isSubmitting = true;
        errorMessage = null;

        try
        {
            var result = await PostsService.CreatePostAsync(postModel);
            if (result.Success)
            {
                NavigationManager.NavigateTo("/posts");
            }
            else
            {
                errorMessage = result.Error ?? "Failed to create post. Please try again.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/posts");
    }
}
