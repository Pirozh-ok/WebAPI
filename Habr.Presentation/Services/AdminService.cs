using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.Presentation.Services
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;

        public AdminService(DataContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegisterAdminAsync(string name, string email, string password)
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
                    Role = Roles.Admin
                });

            await _context.SaveChangesAsync();
            _logger.LogInformation($"\"{name}\" {LogResources.UserRegistered}");
        }
    }
}
