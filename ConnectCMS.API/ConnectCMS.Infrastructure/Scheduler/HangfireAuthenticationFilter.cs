using Hangfire.Dashboard;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging.Abstractions;

namespace CMSTrain.Infrastructure.Scheduler;

public class HangfireAuthenticationFilter(ILogger logger) : IDashboardAuthorizationFilter
{
    private const string AuthenticationScheme = "Basic";

    public string User { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public HangfireAuthenticationFilter() : this(new NullLogger<HangfireAuthenticationFilter>())
    {
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        var header = httpContext.Request.Headers.Authorization;

        if (MissingAuthorizationHeader(header))
        {
            logger.LogInformation("Authorization Header is missing in the request.");
            
            SetChallengeResponse(httpContext);
            
            return false;
        }

        var authValues = AuthenticationHeaderValue.Parse(header!);

        if (NotBasicAuthentication(authValues))
        {
            logger.LogInformation("The following process could not requested for a basic authentication.");
            
            SetChallengeResponse(httpContext);
            
            return false;
        }

        var tokens = ExtractAuthenticationTokens(authValues);

        if (tokens.AreInvalid())
        {
            logger.LogInformation("Please provide a valid authentication token value.");
            
            SetChallengeResponse(httpContext);
            
            return false;
        }

        if (tokens.CredentialsMatch(User, Password))
        {
            logger.LogInformation("Awesome, the authentication tokens match the configuration.");
            
            return true;
        }

        logger.LogInformation($"Sad, but the authentication tokens for username - {tokens.Username} and password of [{tokens.Password}] do not match configuration.");

        SetChallengeResponse(httpContext);
        
        return false;
    }

    private static bool MissingAuthorizationHeader(StringValues header)
    {
        return string.IsNullOrWhiteSpace(header);
    }

    private static BasicAuthenticationTokens ExtractAuthenticationTokens(AuthenticationHeaderValue authValues)
    {
        var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter!));
        
        var parts = parameter.Split(':');
        
        return new BasicAuthenticationTokens(parts);
    }

    private static bool NotBasicAuthentication(AuthenticationHeaderValue authValues)
    {
        return !AuthenticationScheme.Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase);
    }

    private static void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}

public class BasicAuthenticationTokens(string[] tokens)
{
    public string Username => tokens[0];
    
    public string Password => tokens[1];

    public bool AreInvalid()
    {
        return ContainsTwoTokens() && ValidTokenValue(Username) && ValidTokenValue(Password);
    }

    public bool CredentialsMatch(string user, string pass)
    {
        return Username.Equals(user) && Password.Equals(pass);
    }

    private static bool ValidTokenValue(string token)
    {
        return string.IsNullOrWhiteSpace(token);
    }

    private bool ContainsTwoTokens()
    {
        return tokens.Length == 2;
    }
}