using Newtonsoft.Json;
using CMSTrain.Application.Common.Response;

namespace CMSTrain.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;

        ResponseDto<object> problem;

        switch (ex)
        {
            case BadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                var validationErrorsMessage = badRequestException.ValidationErrors.Aggregate("", (current, exception) => current + exception);

                problem = new ResponseDto<object>
                {
                    StatusCode = (int)statusCode,
                    Message = $"{badRequestException.Message} {validationErrorsMessage}",
                    Result = null
                };
                break;

            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                problem = new ResponseDto<object>
                {
                    StatusCode = (int)statusCode,
                    Message = notFoundException.Message,
                    Result = null,
                };
                break;
            
            case CustomException customException:
                statusCode = HttpStatusCode.InternalServerError;
                problem = new ResponseDto<object>
                {
                    StatusCode = (int)statusCode,
                    Message = customException.Message,
                    Result = null,
                };
                break;
            
            case PartialException partialException:
                statusCode = HttpStatusCode.Accepted;
                problem = new ResponseDto<object>
                {
                    StatusCode = (int)statusCode,
                    Message = partialException.Message,
                    Result = null,
                };
                break;

            default:
                problem = new ResponseDto<object>
                {
                    Message = ex.Message,
                    StatusCode = (int)statusCode,
                    Result = null,
                };
                break;
        }

        httpContext.Response.StatusCode = (int)statusCode;

        var logMessage = JsonConvert.SerializeObject(problem);

        logger.LogError(logMessage);

        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}
