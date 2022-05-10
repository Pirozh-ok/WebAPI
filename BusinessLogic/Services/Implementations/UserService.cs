using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }
        public User LogIn(string email, string password)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            return user;
        }
        public void Register(string name, string email, string password)
        {
            if (_context.Users
                .SingleOrDefault(u => u.Email == email) != null)
            {
                throw new Exception("Пользователь с таким адресом электронной почты уже существует!");
            }

            _context.Users.Add(
                new User
                {
                    Email = email,
                    Name = name,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });

            _context.SaveChanges();
        }

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
    }
}
