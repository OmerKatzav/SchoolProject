namespace Shared;

public class RegisterData
{
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public DateTime Expiration { get; init; }
    public byte[]? Nonce { get; init; }
}