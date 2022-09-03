using Habr.Common;

namespace Habr.Presentation.Extensions
{
    static public class RegisterManagersExtensions
    {
        static public void AddFileManager(this IServiceCollection services)
        {
            services.AddScoped<IFileManager, FileManager>(); 
        }
    }
}
