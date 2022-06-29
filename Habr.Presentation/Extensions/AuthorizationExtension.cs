using System.Text;
using Habr.Presentation.Auth;
using Habr.Presentation.Services;
using Microsoft.IdentityModel.Tokens;

namespace Habr.Presentation.Extensions
{
    static public class AuthorizationExtension
    {
        static public void AddJwtAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Jwt));
            services.AddScoped<IJwtService, JwtService>();
        }
    }
}
