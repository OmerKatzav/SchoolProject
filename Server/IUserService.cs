namespace Server;

internal interface IUserService
{
    public Task<int> GetUserCountAsync();
    public Task<Guid> GetUserIdByUsernameAsync(string username);
    public Task<Guid> GetUserIdByEmailAsync(string email);
    public Task<bool> IsUsernameTakenAsync(string username);
    public Task<bool> IsEmailTakenAsync(string email);
    public Task<bool> IsUserIdTakenAsync(Guid userId);
    public Task<DateTime> GetPasswordLastChangedAsync(Guid userId);
    public Task<bool> IsValidAsync(Guid userId, string password);
    public Task<string> GetUsernameAsync(Guid userId);
    public Task<string> GetEmailAsync(Guid userId);
    public Task SetUsernameAsync(Guid userId, string username);
    public Task SetEmailAsync(Guid userId, string email);
    public Task SetPasswordAsync(Guid userId, string password);
    public (byte[], byte[]) HashPassword(string password);
    public Task<Guid> AddUserAsync(string username, string password, string email, bool isAdmin);
    public Task RemoveUserAsync(Guid userId);
    public Task<bool> IsAdminAsync(Guid userId);
}