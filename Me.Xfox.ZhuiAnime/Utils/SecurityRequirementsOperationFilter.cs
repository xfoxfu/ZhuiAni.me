using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any() ||
            (context.MethodInfo.DeclaringType
                ?.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any()
            ?? true);
        if (allowAnonymous)
        {
            operation.Security = new List<OpenApiSecurityRequirement>() { new() };
        }
    }
}
