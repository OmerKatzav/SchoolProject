using System.ComponentModel.DataAnnotations;

namespace Server
{
    internal class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required, StringLength(16, MinimumLength = 3)]
        public required string Username { get; set; }
        [Required, Length(32, 32)]
        public required byte[] PasswordHash { get; set; }
        [Required, Length(16, 16)]
        public required byte[] PasswordSalt { get; set; }
        [Required, StringLength(254)]
        public required string Email { get; set; }
    }
}
