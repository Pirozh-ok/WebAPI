using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO> LogInAsync(string email, string password);
        public Task<UserDTO> RegisterAsync(string name, string email, string password);
        public Task<UserDTO> GetUserById(int id);
        public Task<IEnumerable<UserDTO>> GetUsersAsync();
        public Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync();
        public Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync();
        public Task DeleteAsync(int id);
        public Task UpdateAsync(int userId, UpdateUserDTO user);
    }
}
