using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("password_salt")]
        public string PasswordSalt { get; set; } = string.Empty;
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("country")]
        public string Country { get; set; } = string.Empty;
        [Column("city")]
        public string City { get; set; } = string.Empty;
        [Column("gender")]
        public string Gender { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
