using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Habr.Presentation.Extensions
{
    public static class SwaggerBuilderExtension
    {
        public static WebApplication UseSwaggerWithVersioning(this WebApplication app)
        {
            IServiceProvider services = app.Services;
            var provider = services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }
}
