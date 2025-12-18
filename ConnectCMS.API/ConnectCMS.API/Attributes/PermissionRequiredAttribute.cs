using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMSTrain.Attributes;

public class PermissionRequiredAttribute(string permission) : AuthorizeAttribute, IAuthorizationFilter
{
    private string Permission { get; } = permission;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            var roles = context.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            bool roleWithPermission = roles.Any(role =>
                context.HttpContext.User.HasClaim(c => c.Type == "Permission" && c.Value == Permission));

            if (!roleWithPermission)
            {
                context.Result = new ForbidResult();
            }

            return;
        }

        context.Result = new UnauthorizedResult();
    }
}