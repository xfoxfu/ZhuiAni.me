using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
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
        [property:JsonPropertyName("trace_id")]
        string TraceId,
        [property:JsonExtensionData]
        IDictionary<string, object> ExtraData
    );

    public record ErrorDevResponse(
        [property:JsonPropertyName("error_code")]
        string ErrorCode,
        [property:JsonPropertyName("message")]
        string Message,
        [property:JsonPropertyName("trace_id")]
        string TraceId,
        [property:JsonPropertyName("stack_trace")]
        string StackTrace,
        [property:JsonExtensionData]
        IDictionary<string, object> ExtraData
    );

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
}

