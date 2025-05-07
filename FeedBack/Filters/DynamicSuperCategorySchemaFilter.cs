
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;
using FeedBack.Data.Models;

namespace FeedBack.Filters
{
    public class DynamicSuperCategorySchemaFilter : ISchemaFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicSuperCategorySchemaFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ReqAddProduct))
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FeedBackContext>();

                var superCategories = db.superCategories
                    .Select(c => c.Name)
                    .ToList();

                if (schema.Properties.TryGetValue("SuperCategoryId", out var superCategoryProperty))
                {
                    superCategoryProperty.Enum = superCategories
                        .Select(c => new OpenApiString(c))
                        .Cast<IOpenApiAny>()
                        .ToList();
                }
            }
        }
    }
}
