using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger; 

        public UserService(DataContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            GuardAgainstInvalidUser(user);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            GuardAgainstInvalidUser(user);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgaunstInvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync()
        {
            var users =  await _context.Users
                .ProjectTo<UserWithCommentsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgaunstInvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserWithPostsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            GuardAgaunstInvalidListUsers(users);
            return users;
        }

        public async Task<RegistrationOrLoginUserDTO> LogInAsync(string email, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            _logger.LogInformation($"\"{user.Name}\" {LogResources.UserLogIn}");
            return _mapper.Map<RegistrationOrLoginUserDTO>(user);
        }

        public async Task<RegistrationOrLoginUserDTO> RegisterAsync(string name, string email, string password)
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
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });

            await _context.SaveChangesAsync();
            _logger.LogInformation($"\"{name}\" {LogResources.UserRegistered}");

            return _mapper.Map<RegistrationOrLoginUserDTO>(_context.Users
                                .SingleOrDefault(u => u.Email == email));
        }

        public async Task UpdateAsync(int userId, UpdateUserDTO user)
        {
            var userToUpdate = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            GuardAgainstInvalidUser(userToUpdate);

            if (user.Email is not null && userToUpdate.Email != user.Email && await _context.Users
                                                        .SingleOrDefaultAsync(u => u.Email == user.Email) != null)
            {
                throw new BusinessException(UserExceptionMessageResource.EmailExists);
            }

            if (!BCrypt.Net.BCrypt.Verify(user.OldPassword, userToUpdate.Password))
            {
                throw new BusinessException(UserExceptionMessageResource.IncorrectPassword);
            }

            userToUpdate.Name = !string.IsNullOrEmpty(user.Name) ? user.Name : userToUpdate.Name;
            userToUpdate.Email = !string.IsNullOrEmpty(user.Email) ? user.Email : userToUpdate.Email;
            userToUpdate.Password = !string.IsNullOrEmpty(user.NewPassword) ?
                                        BCrypt.Net.BCrypt.HashPassword(user.NewPassword) :
                                        userToUpdate.Password;


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

        private void GuardAgaunstInvalidListUsers<T>(IEnumerable<T> users)
        {
            if(users is null || users.Count() == 0)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }
    }
}
