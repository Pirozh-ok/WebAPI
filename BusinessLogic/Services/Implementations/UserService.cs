using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Logging;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger; 
        public UserService(DataContext context, IMapper mapper, ILoggerManager logger)
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
        public async Task<UserDTO> LogInAsync(string email, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            GuardAgainstInvalidUser(user, password);
            _logger.LogInfo($"User \"{user.Name}\" is signed in.");
            return _mapper.Map<UserDTO>(user);
        }
        public async Task RegisterAsync(string name, string email, string password)
        {
            if (await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email) != null)
            {
                throw new ValidationException(Common.Resources.UserExceptionMessageResource.EmailExists);
            }

            await _context.Users.AddAsync(
                new User
                {
                    Email = email,
                    Name = name,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                });

            await _context.SaveChangesAsync();
            _logger.LogInfo($"User \"{name}\" is registered.");
        }
        public async Task UpdateAsync(User user)
        {
            var userToUpdate = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == user.Id);

            GuardAgainstInvalidUser(userToUpdate);

            userToUpdate.Name = user.Name;
            userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(userToUpdate.Password);
            userToUpdate.Email = user.Email;
            userToUpdate.Posts = user.Posts;
            userToUpdate.Comments = user.Comments;

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }
        private void GuardAgainstInvalidUser(User user, string password)
        {
            if (user == null)
            {
                throw new AuthenticationException(Common.Resources.UserExceptionMessageResource.InvalidEmail);
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new AuthenticationException(Common.Resources.UserExceptionMessageResource.IncorrectPassword);
            }
        }
        private void GuardAgainstInvalidUser(User user)
        {
            if(user is null)
            {
                throw new NotFoundException(Common.Resources.UserExceptionMessageResource.UserNotFound);
            }
        }

        private void GuardAgaunstInvalidListUsers<T>(IEnumerable<T> users)
        {
            if(users is null || users.Count() == 0)
            {
                throw new NotFoundException(Common.Resources.UserExceptionMessageResource.UserNotFound);
            }
        }
    }
}
