using System.Security.Claims;

namespace Shared.Core.Authentication;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Lấy userId (Guid string) từ claim sub/nameidentifier của JWT.</summary>
    public static string? GetUserId(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? user.FindFirstValue("sub");

    /// <summary>Lấy userId, ném lỗi nếu không có (dùng trong controller [Authorize]).</summary>
    public static string GetRequiredUserId(this ClaimsPrincipal user)
        => user.GetUserId()
           ?? throw new UnauthorizedAccessException("Token không chứa userId.");
}
