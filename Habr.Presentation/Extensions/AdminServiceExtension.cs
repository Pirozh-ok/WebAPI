using Habr.Presentation.Services;

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
