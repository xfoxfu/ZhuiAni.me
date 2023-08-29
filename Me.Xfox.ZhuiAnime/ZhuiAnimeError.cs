using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Me.Xfox.ZhuiAnime;

public class ZhuiAnimeError : Exception
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

    public class ErrorResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var errorres = new OpenApiResponse
            {
                Description = "Internal server error.",
            };
            var schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorProdResponse), context.SchemaRepository);
            errorres.Content.Add("application/json", new OpenApiMediaType { Schema = schema });
            operation.Responses.Add($"{(int)HttpStatusCode.InternalServerError}", errorres);
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

            error.ExtraData.Add("connection_id", context.HttpContext.Connection.Id);
            error.ExtraData.Add("request_id", context.HttpContext.TraceIdentifier);
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

    public class InternalServerError : ZhuiAnimeError
    {
        public InternalServerError(Exception innerException) : base(
            HttpStatusCode.InternalServerError,
            "INTERNAL_SERVER_ERROR",
            $"An internal server error occurred: {innerException.Message}",
            innerException)
        { }
    }
    public class EndpointNotFound : ZhuiAnimeError
    {
        public EndpointNotFound() : base(HttpStatusCode.NotFound, "ENDPOINT_NOT_FOUND", "API endpoint not found.") { }
    }

    public class BadRequest : ZhuiAnimeError
    {
        public BadRequest(ModelStateDictionary state) : base(
            HttpStatusCode.BadRequest,
            "BAD_REQUEST",
            "Request body is invalid.")
        {
            ExtraData.Add("errors", state
                .Select(e => new { e.Key, Value = e.Value?.Errors.Select(e => e.ErrorMessage) })
                .ToDictionary(e => e.Key, e => e.Value));
        }
    }

    public class CategoryNotFound : ZhuiAnimeError
    {
        public CategoryNotFound(uint id) : base(
            HttpStatusCode.NotFound,
            "CATAGORY_NOT_FOUND",
            $"Catagory {id} not found.")
        {
            ExtraData.Add("category_id", id);
        }
    }

    public class ItemNotFound : ZhuiAnimeError
    {
        public ItemNotFound(uint id) : base(
            HttpStatusCode.NotFound,
            "ITEM_NOT_FOUND",
            $"Item {id} not found.")
        {
            ExtraData.Add("item_id", id);
        }
    }

    public class LinkNotFound : ZhuiAnimeError
    {
        public LinkNotFound(uint id) : base(
            HttpStatusCode.NotFound,
            "LINK_NOT_FOUND",
            $"Link {id} not found.")
        {
            ExtraData.Add("link_id", id);
        }
    }

    public class PikPakJobNotFound : ZhuiAnimeError
    {
        public PikPakJobNotFound(uint id) : base(
            HttpStatusCode.NotFound,
            "PIKPAK_JOB_NOT_FOUND",
            $"PikPak job {id} not found.")
        {
            ExtraData.Add("pikpak_job_id", id);
        }
    }

    public class UserNotFound : ZhuiAnimeError
    {
        public UserNotFound(uint id) : base(
            HttpStatusCode.NotFound,
            "USER_NOT_FOUND",
            $"User {id} not found.")
        {
            ExtraData.Add("user_id", id);
        }
    }

    public class UsernameTaken : ZhuiAnimeError
    {
        public UsernameTaken(string username) : base(
            HttpStatusCode.NotFound,
            "USERNAME_TAKEN",
            $"Username {username} is already taken.")
        {
            ExtraData.Add("username", username);
        }
    }

    public class CaptchaInvalid : ZhuiAnimeError
    {
        public CaptchaInvalid(string token, IEnumerable<string> codes) : base(
            HttpStatusCode.BadRequest,
            "CAPTACHA_INVALID",
            $"Turnstile validation token is invalid since {string.Join(",", codes)}.")
        {
            ExtraData.Add("token", token);
            ExtraData.Add("codes", string.Join(",", codes));
        }
    }

    public class InvalidGrantType : ZhuiAnimeError
    {
        public InvalidGrantType(string grantType) : base(
            HttpStatusCode.BadRequest,
            "INVALID_GRANT_TYPE",
            $"Invalid grant type {grantType}.")
        {
            ExtraData.Add("grant_type", grantType);
        }
    }

    public class InvalidUsernameOrPassword : ZhuiAnimeError
    {
        public InvalidUsernameOrPassword(string username) : base(
            HttpStatusCode.Forbidden,
            "INVALID_USERNAME_OR_PASSWORD",
            $"Invalid username {username} or password.")
        {
            ExtraData.Add("username", username);
        }
    }
}
