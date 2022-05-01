using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private void GuardAgainstInvalidUser(User user, string password)
        {
            if (user == null)
            {
                throw new Exception("Неверный адрес электронной почты!");
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Неверный пароль!");
            }
        }
        public User LogIn(string email, string password)
        {
            using var context = new DataContext();
            var user = context.Users
                .SingleOrDefault(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            return user;
        }
        public void Register(string name, string email, string password)
        {
            using var context = new DataContext();

            if (context.Users
                .SingleOrDefault(u => u.Email == email) != null)
            {
                throw new Exception("Пользователь с таким адресом электронной почты уже существует!");
            }

            context.Users.Add(
                new User
                {
                    Email = email,
                    Name = name,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });

            context.SaveChanges();
        }
    }
}
