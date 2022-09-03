using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Extensions;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager; 

        public UserService(DataContext context, IMapper mapper, ILogger<UserService> logger, IFileManager fileManager)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileManager = fileManager; 
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            GuardAgainstInvalidUser(user);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            GuardAgainstInvalidUser(user);

            var dto = _mapper.Map<UserDTO>(user);
            dto.Avatar = await _fileManager.LoadFile(user.AvatarPath);
            return dto; 
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgainstInvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync()
        {
            var users =  await _context.Users
                .ProjectTo<UserWithCommentsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgainstInvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserWithPostsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgainstInvalidListUsers(users);
            return users;
        }

        public async Task<IdentityDTO> SignInAsync(string email, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            _logger.LogInformation($"\"{user.Name}\" {LogResources.UserLogIn}");
            return _mapper.Map<IdentityDTO>(user);
        }

        public async Task<IdentityDTO> SignUpAsync(string name, string email, string password)
        {
            if (await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email) != null)
            {
                throw new BusinessException(UserExceptionMessageResource.EmailExists);
            }

            await _context.Users.AddAsync(
                new User
                {
                    Email = email,
                    Name = name,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = Roles.User
                });

            await _context.SaveChangesAsync();
            _logger.LogInformation($"\"{name}\" {LogResources.UserRegistered}");

            return _mapper.Map<IdentityDTO>(_context.Users
                                .SingleOrDefault(u => u.Email == email));
        }

        public async Task UpdateAsync(int userId, UpdateUserDTO newUserData)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            GuardAgainstInvalidUser(user);
            GuardAgainstInvalidUserUpdateData(newUserData);
            await GuardAgainstInvalidEmailForUpdate(user.Email, newUserData.Email);
            GuardAgainstInvalidOldPasswordForUpdate(newUserData.OldPassword, user.Password);

            user.Name = string.IsNullOrEmpty(newUserData.Name) ? user.Name : newUserData.Name;
            user.Email = string.IsNullOrEmpty(newUserData.Email) ? user.Email : newUserData.Email;
            user.Password = string.IsNullOrEmpty(newUserData.NewPassword) ?
                                        user.Password : BCrypt.Net.BCrypt.HashPassword(newUserData.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAvatarAsync(int userId, IFormFile newAvatar)
        {
            GuardAgainstInvalidImage(newAvatar);

            var userToUpdate = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            GuardAgainstInvalidUser(userToUpdate);

            var newAvatarPath = await _fileManager.SaveFile(newAvatar, userId);
            userToUpdate.AvatarPath = newAvatarPath;

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }

        private void GuardAgainstInvalidUser(User user, string password)
        {
            if (user == null)
            {
                throw new AuthenticationException(UserExceptionMessageResource.InvalidEmail);
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new AuthenticationException(UserExceptionMessageResource.IncorrectPassword);
            }
        }

        private void GuardAgainstInvalidUser(User user)
        {
            if(user is null)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }

        private void GuardAgainstInvalidListUsers<T>(IEnumerable<T> users)
        {
            if(users is null || users.Count() == 0)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }

        private void GuardAgainstInvalidImage(IFormFile image)
        {
            if (image is null)
            {
                throw new BadRequestException(WorkWithContentExceptionMessageResource.ContentIsNull);
            }

            if(!image.IsImage())
            {
                throw new BadImageFormatException(WorkWithContentExceptionMessageResource.FileIsNotImage);
            }

        }

        private async Task GuardAgainstInvalidEmailForUpdate(string oldEmail, string newEmail)
        {
            if(string.IsNullOrEmpty(newEmail))
            {
                throw new BusinessException(UserExceptionMessageResource.InvalidEmail);
            }

            if (await _context.Users
                                   .SingleOrDefaultAsync(u => u.Email == newEmail) is not null)
            {
                throw new BusinessException(UserExceptionMessageResource.EmailExists);
            }
        }

        private void GuardAgainstInvalidUserUpdateData(UpdateUserDTO userData)
        {
            if (userData is null)
            {
                throw new BadRequestException(UserExceptionMessageResource.UncorrectDataForUpdateUser);
            }
        }

        private void GuardAgainstInvalidOldPasswordForUpdate(string password, string encryptedPassword)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, encryptedPassword))
            {
                throw new BusinessException(UserExceptionMessageResource.IncorrectPassword);
            }
        }
    }
}
