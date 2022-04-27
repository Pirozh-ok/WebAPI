using Habr.DataAccess.Entities;
using Habr.DataAccess.Services.Interfaces;

namespace Habr.DataAccess.Services
{
    public class UserService : IUserService
    {
        public User Login(string email, string password)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Email == email );

            if (user == null || BCrypt.Net.BCrypt.Verify( password, user.Password ))
            {
                throw new Exception( "Неверный логин или пароль!" );
            }

            return user;
        }

        public void Register(string name, string email, string password)
        {
            using var context = new DataContext();

            if (context.Users.SingleOrDefault( u => u.Email == email ) != null)
            {
                throw new Exception( "Пользователь с таким адрессом электронной почты уже существует!" );
            }

            context.Users.Add( new User
            {
                Email = email,
                Name = name,
                Password = BCrypt.Net.BCrypt.HashPassword( password )
            } );

            context.SaveChanges();
        }
    }
}
