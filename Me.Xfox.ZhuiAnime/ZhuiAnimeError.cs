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
        private readonly IHostEnvironment _hostEnvironment;

        public ErrorExceptionFilter(IHostEnvironment hostEnvironment) =>
            _hostEnvironment = hostEnvironment;

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
            context.Result = NormalizeError(
                context.Exception,
                context,
                _hostEnvironment.IsProduction());
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
}
