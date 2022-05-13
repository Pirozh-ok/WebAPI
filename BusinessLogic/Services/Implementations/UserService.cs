using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }
        public async Task<User> LogInAsync(string email, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            return user;
        }
        public async void RegisterAsync(string name, string email, string password)
        {
            if (await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email) != null)
            {
                throw new Exception("A user with this email address already exists!");
            }

            await _context.Users.AddAsync(
                new User
                {
                    Email = email,
                    Name = name,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });

            await _context.SaveChangesAsync();
        }
        private void GuardAgainstInvalidUser(User user, string password)
        {
            if (user == null)
            {
                throw new Exception("Invalid email!");
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Incorrect password!");
            }
        }
    }
}
