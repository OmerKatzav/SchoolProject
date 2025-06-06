using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Server;

internal class UserService(AppDbContext dbContext, ICryptoService cryptoService) : IUserService
{
    public async Task<int> GetUserCountAsync()
    {
        return await dbContext.Users.CountAsync();
    }

    public async Task<Guid> GetUserIdByUsernameAsync(string username)
    {
        return await dbContext.Users
            .Where(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
            .Select(u => u.Id)
            .FirstAsync();
    }

    public async Task<Guid> GetUserIdByEmailAsync(string email)
    {
        return await dbContext.Users
            .Where(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            .Select(u => u.Id)
            .FirstAsync();
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        return await dbContext.Users.AnyAsync(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await dbContext.Users.AnyAsync(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<bool> IsUserIdTakenAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId) != null;
    }

    public async Task<DateTime> GetPasswordLastChangedAsync(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        return user.PasswordLastChanged;
    }

    public async Task<bool> IsValidAsync(Guid userId, string password)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        return user.PasswordHash == cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), user.PasswordSalt, user.Id.ToByteArray());
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
        if (await IsUsernameTakenAsync(username)) throw new ArgumentException($"Username {username} is already taken");
        user.Username = username;
        await dbContext.SaveChangesAsync();
    }

    public async Task SetEmailAsync(Guid userId, string email)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        if (await IsEmailTakenAsync(email)) throw new ArgumentException($"Email {email} is already taken");
        user.Email = email;
        await dbContext.SaveChangesAsync();
    }

    public async Task SetPasswordAsync(Guid userId, string password)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        var salt = cryptoService.GenerateSalt().ToArray();
        user.PasswordHash = [.. cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), salt, user.Id.ToByteArray())];
        user.PasswordSalt = salt;
        user.PasswordLastChanged = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    public (byte[], byte[]) HashPassword(string password)
    {
        var salt = cryptoService.GenerateSalt().ToArray();
        var hash = cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), salt, Guid.NewGuid().ToByteArray()).ToArray();
        return (hash, salt);
    }

    public async Task<Guid> AddUserAsync(string username, string password, string email, bool isAdmin)
    {
        var id = Guid.NewGuid();
        while (await IsUserIdTakenAsync(id)) id = Guid.NewGuid();
        var salt = cryptoService.GenerateSalt().ToArray();
        var hash = cryptoService.HashBytes(Encoding.UTF8.GetBytes(password), salt, id.ToByteArray()).ToArray();
        var user = new User
        {
            Id = id,
            Username = username,
            Email = email,
            PasswordSalt = salt,
            PasswordHash = hash,
            PasswordLastChanged = DateTime.UtcNow,
            IsAdmin = isAdmin
        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user.Id;
    }

    public async Task RemoveUserAsync(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsAdminAsync(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId) ?? throw new ArgumentException($"User with guid {userId} doesn't exist");
        return user.IsAdmin;
    }
}