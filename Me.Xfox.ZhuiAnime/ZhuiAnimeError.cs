using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Me.Xfox.ZhuiAnime;

public abstract class ZAError : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public string ErrorCode { get; set; }

    public IDictionary<string, object> ExtraData { get; set; }

    public ZAError(
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

    public ZAError(
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

    protected ZAError(
        string message,
        Exception? innerException = null) : this(
            message,
            new Dictionary<string, object>(),
            innerException)
    {
    }

    protected ZAError(
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
        public string MessageExample { get; set; }

        public ErrorAttribute(HttpStatusCode statusCode, string errorCode, string messageExample = "")
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            MessageExample = messageExample;
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
        public ErrorProdResponse(ZAError e) : this(
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
        public ErrorDevResponse(ZAError e) : this(
              e.ErrorCode,
              e.Message,
              e.StackTrace ?? e.InnerException?.StackTrace ?? "No stacktrace provided.",
              e.ExtraData)
        {
        }
    };

    [AttributeUsage(AttributeTargets.Method)]
    public class HasAttribute : Attribute
    {
        public IEnumerable<Type> ExceptionTypes { get; set; }
        public HasAttribute(params Type[] exceptionTypes) =>
            ExceptionTypes = exceptionTypes;
    }

    public class ErrorResponsesOperationFilter : IOperationFilter
    {
        private ErrorAttribute GetErrorAttribute(Type type) =>
            (ErrorAttribute)type.GetCustomAttributes(typeof(ErrorAttribute), true).FirstOrDefault()!;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var exceptions = context.MethodInfo
                .GetCustomAttributes(typeof(HasAttribute), true)
                .SelectMany(x => ((HasAttribute)x).ExceptionTypes)
                .Select(GetErrorAttribute)
                .Concat((IEnumerable<ErrorAttribute>)new[]
                {
                    GetErrorAttribute(typeof(InternalServerError)),
                    // if the security is empty, the operation uses the global policy (aka. required auth)
                    operation.Security.Count == 0 ? GetErrorAttribute(typeof(InvalidToken)) : null,
                }.Where(x => x != null))
                .GroupBy(x => x.StatusCode)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());
            foreach (var exception in exceptions)
            {
                operation.Responses.Add(((int)exception.Key).ToString(), new OpenApiResponse
                {
                    Description = string.Join(", ", exception.Value.Select(x => x.ErrorCode)),
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Examples = exception.Value.ToDictionary(x => x.ErrorCode, x => new OpenApiExample
                            {
                                Value = OpenApiAnyFactory.CreateFromJson(JsonSerializer.Serialize(new
                                {
                                    error_code = x.ErrorCode,
                                    message = x.MessageExample,
                                })),
                            })
                        },
                    }
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
                ZAError e => e,
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
            if (context.Exception is Exception e and not ZAError)
            {
                Logger.Error(e, "Internal error occurred.");
            }

            context.Result = NormalizeError(
                context.Exception,
                context,
                HostEnvironment.IsProduction());
        }
    }

    [Error(HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "An internal server error occurred.")]
    public class InternalServerError : ZAError
    {
        public InternalServerError(Exception innerException) : base(
            $"An internal server error occurred: {innerException.Message}",
            innerException)
        { }
    }

    [Error(HttpStatusCode.NotFound, "ENDPOINT_NOT_FOUND", "API endpoint not found.")]
    public class EndpointNotFound : ZAError
    {
        public EndpointNotFound() : base("API endpoint not found.") { }
    }

    [Error(HttpStatusCode.BadRequest, "BAD_REQUEST", "Request body is invalid.")]
    public class BadRequest : ZAError
    {
        public BadRequest(ModelStateDictionary state) : base(
            "Request body is invalid.")
        {
            ExtraData.Add("errors", state
                .Select(e => new { e.Key, Value = e.Value?.Errors.Select(e => e.ErrorMessage) })
                .ToDictionary(e => e.Key, e => e.Value));
        }
    }

    [Error(HttpStatusCode.NotFound, "CATAGORY_NOT_FOUND", "Catagory {category_id} not found.")]
    public class CategoryNotFound : ZAError
    {
        public CategoryNotFound(uint id) : base(
            $"Catagory {id} not found.")
        {
            ExtraData.Add("category_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "ITEM_NOT_FOUND", "Item {item_id} not found.")]
    public class ItemNotFound : ZAError
    {
        public ItemNotFound(uint id) : base(
            $"Item {id} not found.")
        {
            ExtraData.Add("item_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "LINK_NOT_FOUND", "Link {link_id} not found.")]
    public class LinkNotFound : ZAError
    {
        public LinkNotFound(uint id) : base(
            $"Link {id} not found.")
        {
            ExtraData.Add("link_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "PIKPAK_JOB_NOT_FOUND", "PikPak job {pikpak_job_id} not found.")]
    public class PikPakJobNotFound : ZAError
    {
        public PikPakJobNotFound(uint id) : base(
            $"PikPak job {id} not found.")
        {
            ExtraData.Add("pikpak_job_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "USER_NOT_FOUND", "User {user_id} not found.")]
    public class UserNotFound : ZAError
    {
        public UserNotFound(uint id) : base(
            $"User {id} not found.")
        {
            ExtraData.Add("user_id", id);
        }
    }

    [Error(HttpStatusCode.NotFound, "USERNAME_TAKEN", "Username {username} is already taken.")]
    public class UsernameTaken : ZAError
    {
        public UsernameTaken(string username) : base(
            $"Username {username} is already taken.")
        {
            ExtraData.Add("username", username);
        }
    }

    [Error(HttpStatusCode.BadRequest, "INVALID_CAPTCHA", "Turnstile validation token is invalid since {codes}.")]
    public class InvalidCaptcha : ZAError
    {
        public InvalidCaptcha(string token, IEnumerable<string> codes) : base(
            $"Turnstile validation token is invalid since {string.Join(",", codes)}.")
        {
            ExtraData.Add("token", token);
            ExtraData.Add("codes", string.Join(",", codes));
        }
    }

    [Error(HttpStatusCode.BadRequest, "INVALID_GRANT_TYPE", "Invalid grant type {grant_type}.")]
    public class InvalidGrantType : ZAError
    {
        public InvalidGrantType(string grantType) : base(
            $"Invalid grant type {grantType}.")
        {
            ExtraData.Add("grant_type", grantType);
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_USERNAME_OR_PASSWORD", "Invalid username {username} or password.")]
    public class InvalidUsernameOrPassword : ZAError
    {
        public InvalidUsernameOrPassword(string username) : base(
            $"Invalid username {username} or password.")
        {
            ExtraData.Add("username", username);
        }
    }

    [Error(HttpStatusCode.Unauthorized, "INVALID_TOKEN", "Invalid token {oauth_code}: {oauth_desc}.")]
    public class InvalidToken : ZAError
    {
        public InvalidToken(string? code, string? description, Exception? ex) : base(
            $"Invalid token {code}: {description}.",
            ex)
        {
            ExtraData.Add("oauth_code", code ?? string.Empty);
            ExtraData.Add("oauth_desc", description ?? string.Empty);
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_TOKEN_NOT_FIRST_PARTY", "Token is not issued to first-party application.")]
    public class InvalidTokenNotFirstParty : ZAError
    {
        public InvalidTokenNotFirstParty() : base(
            $"Token is not issued to first-party application.")
        {
        }
    }

    [Error(HttpStatusCode.Forbidden, "INVALID_REFRESH_TOKEN", "Refresh token is not valid for {code}.")]
    public class InvalidRefreshToken : ZAError
    {
        public InvalidRefreshToken(string code) : base(
            $"Refresh token is not valid for {code}.")
        {
            ExtraData.Add("code", code);
        }
    }
}
