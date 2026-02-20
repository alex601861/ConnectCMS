using System.Security.Claims;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Common.User;

public interface ICurrentUserService : ITransientService
{
    bool IsAuthenticated { get; }

    Guid GetUserId { get; }

    string GetUserEmail { get; }

    string GetUserRole { get; }

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}