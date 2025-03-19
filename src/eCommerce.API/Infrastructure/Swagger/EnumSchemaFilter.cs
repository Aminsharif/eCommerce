using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.Serialization;

namespace eCommerce.API.Infrastructure.Swagger
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                foreach (var enumValue in Enum.GetValues(context.Type))
                {
                    schema.Enum.Add(OpenApiAnyFactory.CreateFromJson(enumValue.ToString()));
                }
            }
        }
    }
} 