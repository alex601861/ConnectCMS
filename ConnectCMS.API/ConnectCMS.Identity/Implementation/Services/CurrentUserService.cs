using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CMSTrain.Application.Common.User;

namespace CMSTrain.Identity.Implementation.Services;

public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public bool IsAuthenticated
    {
        get
        {
            var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId != null;
        }
    }
    
    public Guid GetUserId
    {
        get
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaimValue, out var userId) ? userId : Guid.Empty;
        }
    }

    public string GetUserEmail
    {
        get
        {
            var emailAddressClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

            return emailAddressClaimValue ?? string.Empty;
        }
    }

    public string GetUserRole
    {
        get
        {
            var roleClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

            return roleClaimValue ?? string.Empty;
        }
    }

    public bool IsInRole(string role)
    {
        var roleName = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        return roleName != null && roleName == role;
    }

    public IEnumerable<Claim> GetUserClaims()
    {
        var claims = contextAccessor.HttpContext?.User?.Claims;
        
        return claims ?? Enumerable.Empty<Claim>();
    }
}