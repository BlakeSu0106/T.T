using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Telligent.Tag.Application.Swagger;

public class RequiredHeaderParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Tenant",
            In = ParameterLocation.Header,
            AllowEmptyValue = false,
            Description = "tenant id"
        });
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Company",
            In = ParameterLocation.Header,
            AllowEmptyValue = true,
            Description = "company id"
        });
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "User",
            In = ParameterLocation.Header,
            AllowEmptyValue = false,
            Description = "user id"
        });
    }
}