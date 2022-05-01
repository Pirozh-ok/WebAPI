using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        public User LogIn(string email, string password);
        public void Register(string name, string email, string password);
    }
}
