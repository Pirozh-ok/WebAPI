using Habr.Presentation.Services.Implementations;
using Hangfire;

namespace Habr.Presentation.Extensions
{
    public static class HangfireSettings
    {
        public static void AddHangfireServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHangfireServer();
        }

        public static void AddHangfireDashboard(this WebApplication? app)
        {
            app.UseHangfireDashboard();
        }

        public static void StartJobs(this WebApplication? app)
        {
            JobService.StartRecurringJob(); 
        }
    }
}
