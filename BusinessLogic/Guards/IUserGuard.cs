using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Habr.BusinessLogic.Guards
{
    public interface IUserGuard
    {
        void NullArgument<T>(T obj);
        void InvalidUser(User? user);
        void InvalidListUsers<T>(IEnumerable<T> users);
        void InvalidPassword(string encryptedPassword, string password);
        void InvalidPassword(string password);
        Task InvalidEmail(string? email);
        void InvalidName (string? name);
        Task InvalidNewUser(CreateUserDTO newUser);
        void InvalidImage(IFormFile image);
    }
}
