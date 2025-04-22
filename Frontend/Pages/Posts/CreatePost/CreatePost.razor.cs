using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.DTOs.Posts;

namespace Frontend.Pages.Posts.CreatePost;

public partial class CreatePost : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IPostsService PostsService { get; set; } = default!;

    private PostCreation postModel = new();
    private bool isSubmitting = false;
    private string? errorMessage;

    private async Task HandleSubmit()
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

    private void GoBack()
    {
        NavigationManager.NavigateTo("/posts");
    }
}