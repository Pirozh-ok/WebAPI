using Habr.BusinessLogic.Guards.Implementations;
using Habr.BusinessLogic.Guards.Interfaces;
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
            services.AddScoped<IPostGuard, PostGuard>();
            services.AddScoped<ICommentGuard, CommentGuard>();
        }
    }
}
