using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        public User Login(string email, string password);
        public void Register(string name, string email, string password);
    }
}
