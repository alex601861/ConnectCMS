using System.Net;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMSTrain.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TokenHandlerAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = context.HttpContext.Request.Headers["Authorization"];

        string token = accessToken.ToString().Replace("Bearer ", "");

        var tokenManager = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        if (!tokenManager.IsTokenExpired(token)) return;

        var response = new ResponseDto<object>
        {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            Message = "Token has expired",
            Result = null
        };

        var result = new ObjectResult(response)
        {
            StatusCode = (int)HttpStatusCode.Unauthorized
        };

        context.Result = result;
    }
}