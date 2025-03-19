using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eCommerce.API.Infrastructure.Swagger
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            // Check if the endpoint is marked as deprecated
            operation.Deprecated = apiDescription.CustomAttributes().Any(attr => attr.GetType() == typeof(ObsoleteAttribute));

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
                var statusCode = responseType.ApiResponseFormats[0].MediaType;
                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);
                if (response.Value != null)
                {
                    response.Value.Description = responseType.Type?.Name ?? responseType.Type?.FullName;
                }
            }

            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description?.ModelMetadata?.Description;
                }

                parameter.Required |= description?.IsRequired ?? false;
            }
        }
    }
} 