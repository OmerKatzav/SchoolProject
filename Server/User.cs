using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server;

internal class User
{
    [Key, Column("id")]
    public required Guid Id { get; init; }

    [Required, StringLength(16, MinimumLength = 3), Column("username")]
    public required string Username { get; set; }

    [Required, Length(32, 32), Column("password_hash")]
    public required byte[] PasswordHash { get; set; }

    [Required, Length(16, 16), Column("password_salt")]
    public required byte[] PasswordSalt { get; set; }

    [Required, Column("password_last_changed")]
    public required DateTime PasswordLastChanged { get; set; }

    [Required, StringLength(254), Column("email")]
    public required string Email { get; set; }

    [Required, Column("is_admin")]
    public bool IsAdmin { get; set; }
}