using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Posts;

public class CommentCreation
{
    public int ContentId { get; set; }
    [Required(ErrorMessage = "Content is required")]
    [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters")]
    public string Content { get; set; } = string.Empty;
}