using Habr.Presentation.Services.Implementations;
using Habr.Presentation.Services.Interfaces;

namespace Habr.Presentation.Extensions
{
    static public class AdminServiceExtension
    {
        public static void AddAdminService(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
        }
    }
}
