using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core;

[Table("likes")]
public class Like
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("content_id")]
    public int ContentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Content Content { get; set; } = null!;
}