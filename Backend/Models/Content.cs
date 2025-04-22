using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Core;

[Table("contents")]
public class Content
{
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("content")]
    public string ContentText { get; set; } = null!;

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("parent_id")]
    public int? ParentId { get; set; }

    [Column("post_id")]
    public int? PostId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    [ForeignKey("ParentId")]
    public Content? Parent { get; set; }

    public ICollection<Content> Replies { get; set; } = new List<Content>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    [NotMapped]
    public bool IsPost => ParentId == null;

    [NotMapped]
    public bool IsComment => ParentId != null;
}