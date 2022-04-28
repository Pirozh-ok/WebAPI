using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        public User Login(string email, string password)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Email == email );

            if (user == null)
            {
                throw new Exception( "Неверный адрес электронной почты!" );
            }
            else if(!BCrypt.Net.BCrypt.Verify( password, user.Password ))
            {
                throw new Exception( "Неверный пароль!" );
            }

            return user;
        }

        public void Register(string name, string email, string password)
        {
            using var context = new DataContext();

            if (context.Users.SingleOrDefault( u => u.Email == email ) != null)
            {
                throw new Exception( "Пользователь с таким адресом электронной почты уже существует!" );
            }

            var password1 = BCrypt.Net.BCrypt.HashPassword( password );

            context.Users.Add( new User
            {
                Email = email,
                Name = name,
                Password = password1
            } );

            context.SaveChanges();
        }
    }
}
