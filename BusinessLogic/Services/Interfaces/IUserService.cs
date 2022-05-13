using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> LogInAsync(string email, string password);
        public void RegisterAsync(string name, string email, string password);
    }
}
