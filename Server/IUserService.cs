namespace Server
{
    internal interface IUserService
    {
        public Task<Guid> GetUserIdAsync(string username);
        public Task<bool> IsValidAsync(Guid userId, string password);
        public Task<string> GetUsernameAsync(Guid userId);
        public Task<string> GetEmailAsync(Guid userId);
        public Task SetUsernameAsync(Guid userId, string username);
        public Task SetEmailAsync(Guid userId, string email);
        public Task SetPasswordAsync(Guid userId, string password);
        public Task<Guid> AddUserAsync(string username, string password, string email);
    }
}
