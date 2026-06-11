namespace Auth.Application.DTOs;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = string.Empty;
}

public class LoginRequest
{
    /// <summary>Email hoặc username</summary>
    public string EmailOrUsername { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RefreshRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
