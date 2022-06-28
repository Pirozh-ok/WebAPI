using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Habr.Presentation.Extensions
{
    public static class VersioningApiExtension
    {
        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                            new UrlSegmentApiVersionReader(),
                            new HeaderApiVersionReader("api-version"),
                            new QueryStringApiVersionReader("api-version"),
                            new MediaTypeApiVersionReader("version"));
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>,
                ConfigureSwaggerOptions>();
            //services.ConfigureOptions<ConfigureSwaggerOptions>();
        }
    }
}
