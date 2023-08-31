using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Me.Xfox.ZhuiAnime;

public abstract class ZhuiAnimeError : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public string ErrorCode { get; set; }

    public IDictionary<string, object> ExtraData { get; set; }

    public ZhuiAnimeError(
        HttpStatusCode statusCode,
        string errorCode,
        string message,
        Exception? innerException = null) : this(
            statusCode,
            errorCode,
            message,
            new Dictionary<string, object>(),
            innerException)
    {
    }

    public ZhuiAnimeError(
        HttpStatusCode statusCode,
        string errorCode,
        string message,
        IDictionary<string, object> extraData,
        Exception? innerException = null) : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ExtraData = extraData;
    }

    protected ZhuiAnimeError(
        string message,
        Exception? innerException = null) : this(
            message,
            new Dictionary<string, object>(),
            innerException)
    {
    }

    protected ZhuiAnimeError(
        string message,
            IDictionary<string, object> extraData,
        Exception? innerException = null) : base(message, innerException)
    {
        var type = GetType().GetCustomAttributes(typeof(ErrorAttribute), true).FirstOrDefault() as ErrorAttribute ??
            throw new Exception("ErrorAttribute not found, cannot use this constructor.");
        StatusCode = type.StatusCode;
        ErrorCode = type.ErrorCode;
        ExtraData = extraData;
    }

    [AttributeUsage(AttributeTargets.Class)]
    protected class ErrorAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public ErrorAttribute(HttpStatusCode statusCode, string errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public void WithHttpContext(HttpContext context)
    {
        ExtraData.Add("connection_id", context.Connection.Id);
        ExtraData.Add("request_id", context.TraceIdentifier);
    }

    public record ErrorProdResponse(
        [property:JsonPropertyName("error_code")]
        string ErrorCode,
        [property:JsonPropertyName("message")]
        string Message,
        [property:JsonExtensionData]
        IDictionary<string, object> ExtraData
    )
    {
        public ErrorProdResponse(ZhuiAnimeError e) : this(
            e.ErrorCode,
            e.Message,
            e.ExtraData)
        {
        }

    };

    public record ErrorDevResponse(
        [property:JsonPropertyName("error_code")]
        string ErrorCode,
        [property:JsonPropertyName("message")]
        string Message,
        [property:JsonPropertyName("stack_trace")]
        string StackTrace,
        [property:JsonExtensionData]
        IDictionary<string, object> ExtraData
    )
    {
        public ErrorDevResponse(ZhuiAnimeError e) : this(
              e.ErrorCode,
              e.Message,
              e.StackTrace ?? e.InnerException?.StackTrace ?? "No stacktrace provided.",
              e.ExtraData)
        {
        }
    };

    [AttributeUsage(AttributeTargets.Method)]
    public class HasExceptionAttribute : Attribute
    {
        public IEnumerable<Type> ExceptionTypes { get; set; }
        public HasExceptionAttribute(params Type[] exceptionTypes) =>
            ExceptionTypes = exceptionTypes;
    }

    public class ErrorResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var exceptions = context.MethodInfo
                .GetCustomAttributes(typeof(HasExceptionAttribute), true)
                .SelectMany(x => ((HasExceptionAttribute)x).ExceptionTypes)
                .Select(x => (ErrorAttribute)x.GetCustomAttributes(typeof(ErrorAttribute), true).FirstOrDefault()!)
                .Concat((IEnumerable<ErrorAttribute>)new[]
                {
                    new ErrorAttribute(HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR"),
                    operation.Security.Any() ? new ErrorAttribute(HttpStatusCode.Unauthorized, "INVALID_TOKEN") : null,
                }.Where(x => x != null))
                .GroupBy(x => x.StatusCode)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());
            foreach (var exception in exceptions)
            {
                operation.Responses.Add(((int)exception.Key).ToString(), new OpenApiResponse
                {
                    Description = string.Join(", ", exception.Value.Select(x => x.ErrorCode)),
                });
            }
        }
    }

    public class ErrorExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment HostEnvironment;
        private readonly Serilog.ILogger Logger;

        public ErrorExceptionFilter(IHostEnvironment hostEnvironment, Serilog.ILogger logger)
        {
            HostEnvironment = hostEnvironment;
            Logger = logger;
        }

        private static IActionResult NormalizeError(Exception exception, ExceptionContext context, bool isProduction)
        {
            var error = exception switch
            {
                ZhuiAnimeError e => e,
                Exception e => new InternalServerError(e),
                null => new InternalServerError(new Exception("Null exception thrown.")),
            };

            error.WithHttpContext(context.HttpContext);
            error.ExtraData.Add("action_id", context.ActionDescriptor.Id);

            if (isProduction)
            {
                return new ObjectResult(new ErrorProdResponse(error))
                { StatusCode = (int)error.StatusCode };
            }
            else
            {
                return new ObjectResult(new ErrorDevResponse(error))
                { StatusCode = (int)error.StatusCode };
            }
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is Exception e and not ZhuiAnimeError)
            {
                Logger.Error(e, "Internal error occurred.");
            }

            context.Result = NormalizeError(
                context.Exception,
                context,
                HostEnvironment.IsProduction());
        }
    }

    [Error(HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR")]
    public class InternalServerError : ZhuiAnimeError
    {
        public InternalServerError(Exception innerException) : base(
            $"An internal server error occurred: {innerException.Message}",
            innerException)
        { }
    }

    [Error(HttpStatusCode.NotFound, "ENDPOINT_NOT_FOUND")]
    public class EndpointNotFound : ZhuiAnimeError
    {
        public EndpointNotFound() : base("API endpoint not found.") { }
    }

    [Error(HttpStatusCode.BadRequest, "BAD_REQUEST")]
    public class BadRequest : ZhuiAnimeError
    {
        public BadRequest(ModelStateDictionary state) : base(
            "Request body is invalid.")
        {
            ExtraData.Add("errors", state
                .Select(e => new { e.Key, Value = e.Value?.Errors.Select(e => e.ErrorMessage) })
                .ToDictionary(e => e.Key, e => e.Value));
        }
    }

    [Error(HttpStatusCode.NotFound, "CATAGORY_NOT_FOUND")]
    public class CategoryNotFound : ZhuiAnimeError
    {
        public CategoryNotFound(uint id) : base(
            $"Catagory {id} not found.")
        {
            ExtraData.Add("category_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "ITEM_NOT_FOUND")]
    public class ItemNotFound : ZhuiAnimeError
    {
        public ItemNotFound(uint id) : base(
            $"Item {id} not found.")
        {
            ExtraData.Add("item_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "LINK_NOT_FOUND")]
    public class LinkNotFound : ZhuiAnimeError
    {
        public LinkNotFound(uint id) : base(
            $"Link {id} not found.")
        {
            ExtraData.Add("link_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "PIKPAK_JOB_NOT_FOUND")]
    public class PikPakJobNotFound : ZhuiAnimeError
    {
        public PikPakJobNotFound(uint id) : base(
            $"PikPak job {id} not found.")
        {
            ExtraData.Add("pikpak_job_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "USER_NOT_FOUND")]
    public class UserNotFound : ZhuiAnimeError
    {
        public UserNotFound(uint id) : base(
            $"User {id} not found.")
        {
            ExtraData.Add("user_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "USERNAME_TAKEN")]
    public class UsernameTaken : ZhuiAnimeError
    {
        public UsernameTaken(string username) : base(
            $"Username {username} is already taken.")
        {
            ExtraData.Add("username", username);
        }
    }

    [Error(HttpStatusCode.BadRequest, "INVALID_CAPTCHA")]
    public class InvalidCaptcha : ZhuiAnimeError
    {
        public InvalidCaptcha(string token, IEnumerable<string> codes) : base(
            $"Turnstile validation token is invalid since {string.Join(",", codes)}.")
        {
            ExtraData.Add("token", token);
            ExtraData.Add("codes", string.Join(",", codes));
        }
    }

    [Error(HttpStatusCode.BadRequest, "INVALID_GRANT_TYPE")]
    public class InvalidGrantType : ZhuiAnimeError
    {
        public InvalidGrantType(string grantType) : base(
            $"Invalid grant type {grantType}.")
        {
            ExtraData.Add("grant_type", grantType);
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_USERNAME_OR_PASSWORD")]
    public class InvalidUsernameOrPassword : ZhuiAnimeError
    {
        public InvalidUsernameOrPassword(string username) : base(
            $"Invalid username {username} or password.")
        {
            ExtraData.Add("username", username);
        }
    }

    [Error(HttpStatusCode.Unauthorized, "INVALID_TOKEN")]
    public class InvalidToken : ZhuiAnimeError
    {
        public InvalidToken(string? code, string? description, Exception? ex) : base(
            $"Invalid token {code}: {description}.",
            ex)
        {
            ExtraData.Add("auth_code", code ?? string.Empty);
            ExtraData.Add("auth_desc", description ?? string.Empty);
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_TOKEN_NOT_FIRST_PARTY")]
    public class InvalidTokenNotFirstParty : ZhuiAnimeError
    {
        public InvalidTokenNotFirstParty() : base(
            $"Token is not issued to first-party application.")
        {
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_REFRESH_TOKEN")]
    public class InvalidRefreshToken : ZhuiAnimeError
    {
        public InvalidRefreshToken(string code) : base(
            $"Refresh token is not valid for {code}.")
        {
        }
    }
}
