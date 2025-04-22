using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Server
{
    internal class UserService(AppDbContext dbContext, ICryptoService cryptoService) : IUserService
    {
        public async Task<Guid> GetUserIdAsync(string username)
        {

            return await dbContext.Users
               .Where(u => u.Username == username)
               .Select(u => u.Id)
               .FirstAsync();
        }

        public async Task<bool> IsValidAsync(Guid userId, string password)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            return user.PasswordHash == cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), user.PasswordSalt,
                user.Id.ToByteArray());
        }

        public async Task<string> GetUsernameAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            return user.Username;
        }

        public async Task<string> GetEmailAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            return user.Email;
        }

        public async Task SetUsernameAsync(Guid userId, string username)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            user.Username = username;
        }

        public async Task SetEmailAsync(Guid userId, string email)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            user.Email = email;
        }

        public async Task SetPasswordAsync(Guid userId, string password)
        {
            var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
            var salt = cryptoService.GenerateSalt().ToArray();
            user.PasswordHash = [.. cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), salt, user.Id.ToByteArray())];
            user.PasswordSalt = salt;
        }

        public async Task<Guid> AddUserAsync(string username, string password, string email)
        {
            var id = Guid.NewGuid();
            var salt = cryptoService.GenerateSalt().ToArray();
            var hash = cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), salt, id.ToByteArray()).ToArray();
            var user = new User
            {
                Id = id,
                Username = username,
                Email = email,
                PasswordSalt = salt,
                PasswordHash = hash
            };
            await dbContext.Users.AddAsync(user);
            return user.Id;
        }
    }
}
