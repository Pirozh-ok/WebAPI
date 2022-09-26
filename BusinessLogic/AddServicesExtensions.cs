using Habr.BusinessLogic.Guards;
using Habr.BusinessLogic.Services.Implementations;
using Habr.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Habr.BusinessLogic
{
    static public class AddServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserGuard, UserGuard>();
        }
    }
}
