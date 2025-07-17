using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.OData.ModelBuilder;

namespace ProductsApi.Configuration
{
    public class ProductModelConfiguration : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
        {
            var products = builder.EntitySet<Models.Product>("Products").EntityType;
            products.HasKey(p => p.Id);
            products.Property(p => p.Name).IsRequired();
            products.Property(p => p.Price).IsRequired();
        }
    }
}
