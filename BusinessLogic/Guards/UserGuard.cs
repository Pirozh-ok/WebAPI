using System.Net.Mail;
using System.Text.RegularExpressions;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Extensions;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Habr.BusinessLogic.Guards
{
    public class UserGuard : IUserGuard
    {
        private DataContext _context;
        private readonly string emailRegex; 

        public UserGuard(DataContext context, IConfiguration config)
        {
            _context = context;
            emailRegex = config["Mail:Regex"]; 
        }

        public async void InvalidNewUser(CreateUserDTO newUser)
        {
            NullArgument(newUser);
            await InvalidEmail(newUser.Email);
            InvalidPassword(newUser.Password);
            InvalidName(newUser.Name);
        }

        public void NullArgument<T>(T obj)
        {
            if(obj is null)
            {
                throw new BadRequestException(UserExceptionMessageResource.ArgumentIsNull); 
            }
        }

        public void InvalidUser(User? user)
        {
            if (user is null)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }

        public async Task InvalidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase))
            {
                throw new BusinessException(UserExceptionMessageResource.InvalidEmail);
            }

            if (await _context.Users.SingleOrDefaultAsync(u => u.Email == email) is not null)
            {
                 throw new BusinessException(UserExceptionMessageResource.EmailExists);
            }
        }

        public void InvalidPassword(string encryptedPassword, string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || !BCrypt.Net.BCrypt.Verify(password, encryptedPassword))
            {
                throw new BusinessException(UserExceptionMessageResource.InvalidPassword);
            }
        }

        public void InvalidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BusinessException(UserExceptionMessageResource.InvalidPassword);
            }
        }

        public void InvalidName(string? name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessException(UserExceptionMessageResource.InvalidUserName); 
            }
        }

        public void InvalidImage(IFormFile image)
        {
            if (image is null)
            {
                throw new BadRequestException(WorkWithContentExceptionMessageResource.ContentIsNull);
            }

            if (!image.IsImage())
            {
                throw new BadImageFormatException(WorkWithContentExceptionMessageResource.FileIsNotImage);
            }

        }

        public void InvalidListUsers<T>(IEnumerable<T> users)
        {
            if (users is null || users.Count() == 0)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }
    }
}
