using FeedBack.Data.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FeedBack.Filters
{
    public class DynamicCategorySchemaFilter : ISchemaFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicCategorySchemaFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ReqAddProduct))
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FeedBackContext>();

                var categories = db.categories
                    .Select(c => c.Name)
                    .ToList();

                if (schema.Properties.TryGetValue("CategoryId", out var categoryProperty))
                {
                    categoryProperty.Enum = categories
                        .Select(c => new OpenApiString(c))
                        .Cast<IOpenApiAny>()
                        .ToList();
                }
            }
        }
    }
}
