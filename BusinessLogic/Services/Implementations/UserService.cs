using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Guards;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common;
using Habr.Common.DTOs.ImageDTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly IConfiguration _configuration;
        private readonly IUserGuard _guard;  

        public UserService(
            DataContext context, 
            IMapper mapper, 
            ILogger<UserService> logger, 
            IFileManager fileManager, 
            IConfiguration configuration,
            IUserGuard guard)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileManager = fileManager; 
            _configuration = configuration;
            _guard = guard;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            _guard.InvalidUser(user);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

            _guard.InvalidUser(user);
            return _mapper.Map<UserDTO>(user); 
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync()
        {
            var users =  await _context.Users
                .ProjectTo<UserWithCommentsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListUsers(users);
            return users;
        }

        public async Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync()
        {
            var users = await _context.Users
                .ProjectTo<UserWithPostsDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListUsers(users);
            return users;
        }

        public async Task<IdentityDTO> SignInAsync(UserSignInDTO userSignInData)
        {
            _guard.NullArgument(userSignInData); 
            await _guard.InvalidEmail(userSignInData.Email);

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == userSignInData.Email);

            _guard.InvalidPassword(user!.Password, userSignInData.Password);
            _logger.LogInformation($"\"{user.Name}\" {LogResources.UserLogIn}");
            return _mapper.Map<IdentityDTO>(user);
        }

        public async Task<IdentityDTO> SignUpAsync(CreateUserDTO newUser)
        {
            _guard.InvalidNewUser(newUser);

            var user = new User
            {
                Email = newUser.Email,
                Name = newUser.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Role = Roles.User,
                RegistrationDate = DateTime.UtcNow,
                AvatarImage = new AvatarImage()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"\"{newUser.Name}\" {LogResources.UserRegistered}");

            return _mapper.Map<IdentityDTO>(_context.Users
                                .SingleOrDefault(u => u.Email == user.Email));
        }

        public async Task UpdateAsync(int userId, UpdateUserDTO newUserData)
        {
            _guard.NullArgument(newUserData);

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            _guard.InvalidUser(user);

            if(newUserData.Email is not null)
            {
                await _guard.InvalidEmail(newUserData.Email);
                user!.Email = newUserData.Email;
            }

            if (newUserData.NewPassword is not null && newUserData.OldPassword is not null)
            {
                _guard.InvalidPassword(newUserData.OldPassword, newUserData.NewPassword);
                user!.Password = BCrypt.Net.BCrypt.HashPassword(newUserData.NewPassword);
            }

            if (newUserData.Name is not null)
            {
                _guard.InvalidName(newUserData.Email);
                user!.Email = newUserData.Email;
            }

            _context.Users.Update(user!);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAvatarAsync(int userId, IFormFile newAvatar)
        {
            _guard.InvalidImage(newAvatar);

            var userToUpdate = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            _guard.InvalidUser(userToUpdate);

            var newImagePath = await _fileManager.SaveFile(newAvatar, userId);
            userToUpdate!.AvatarImage.PathImage = newImagePath;
            userToUpdate!.AvatarImage.LoadDate = DateTime.Now; 
          
            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task<ImageDTO> GetUserAvatar(int userId)
        {
            var user = await _context.Users
                .Include(u => u.AvatarImage)
                .SingleOrDefaultAsync(u => u.Id == userId);

            _guard.InvalidUser(user);
            return _mapper.Map<ImageDTO>(user.AvatarImage); 
        }        
    }
}
