using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Posts;

public class PostCreation
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot be longer than 5000 characters")]
    public string Content { get; set; } = string.Empty;
}