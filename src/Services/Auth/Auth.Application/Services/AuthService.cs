using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher,
        IJwtTokenService jwt)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        if (await _users.ExistsAsync(email, username))
            throw new InvalidOperationException("Email hoặc username đã tồn tại.");

        if (request.Password.Length < 6)
            throw new InvalidOperationException("Mật khẩu phải có ít nhất 6 ký tự.");

        var user = new User
        {
            Email = email,
            Username = username,
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                ? username
                : request.DisplayName.Trim(),
            PasswordHash = _hasher.Hash(request.Password)
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailOrUsernameAsync(
            request.EmailOrUsername.Trim().ToLowerInvariant())
            ?? await _users.GetByEmailOrUsernameAsync(request.EmailOrUsername.Trim());

        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Sai thông tin đăng nhập.");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        var stored = await _refreshTokens.GetByTokenAsync(request.RefreshToken)
            ?? throw new UnauthorizedAccessException("Refresh token không hợp lệ.");

        if (!stored.IsActive)
            throw new UnauthorizedAccessException("Refresh token đã hết hạn hoặc bị thu hồi.");

        // Rotate: revoke old, issue new
        stored.RevokedAt = DateTime.UtcNow;
        await _refreshTokens.SaveChangesAsync();

        return await IssueTokensAsync(stored.User);
    }

    public async Task LogoutAsync(LogoutRequest request)
    {
        var stored = await _refreshTokens.GetByTokenAsync(request.RefreshToken);
        if (stored != null && stored.RevokedAt == null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokens.SaveChangesAsync();
        }
    }

    public async Task<UserDto?> GetMeAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        return user == null ? null : ToDto(user);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<AuthResponse> IssueTokensAsync(User user)
    {
        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        await _refreshTokens.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = _jwt.GetRefreshTokenExpiry()
        });
        await _refreshTokens.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = expiresAt,
            User = ToDto(user)
        };
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        Username = u.Username,
        DisplayName = u.DisplayName,
        CreatedAt = u.CreatedAt
    };
}
