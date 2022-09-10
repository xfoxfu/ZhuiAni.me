using System;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Me.Xfox.ZhuiAnime;

public class ZhuiAnimeError : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public string ErrorCode { get; set; }

    public ZhuiAnimeError(HttpStatusCode statusCode, string errorCode, string message) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public ZhuiAnimeError(
        HttpStatusCode statusCode,
        string errorCode,
        string message,
        Exception innerException
    ) : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public record ErrorProdResponse(
        [property:JsonPropertyName("error_code")]
        string ErrorCode,
        [property:JsonPropertyName("message")]
        string Message
    );

    public record ErrorDevResponse(
        [property:JsonPropertyName("error_code")]
        string ErrorCode,
        [property:JsonPropertyName("message")]
        string Message,
        [property:JsonPropertyName("stack_trace")]
        string StackTrace
    );

    public class OpenApiErrorProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (!context.SchemaResolver.HasSchema(typeof(ErrorProdResponse), false))
            {
                context.SchemaResolver.AddSchema(
                typeof(ErrorProdResponse),
                false,
                context.SchemaGenerator.Generate(typeof(ErrorProdResponse)));
            }

            context.OperationDescription.Operation.Responses[$"{(int)HttpStatusCode.InternalServerError}"]
                = new NSwag.OpenApiResponse()
                {
                    Schema = context.SchemaResolver.GetSchema(typeof(ErrorProdResponse), false),
                };
            return true;
        }
    }

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

