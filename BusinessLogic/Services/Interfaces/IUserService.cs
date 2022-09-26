using Habr.Common.DTOs.ImageDTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityDTO> SignInAsync(UserSignInDTO userSignInData);
        public Task<IdentityDTO> SignUpAsync(CreateUserDTO newUserData);
        public Task<UserDTO> GetUserByIdAsync(int id);
        public Task<IEnumerable<UserDTO>> GetUsersAsync();
        public Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync();
        public Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync();
        public Task DeleteAsync(int id);
        public Task UpdateAsync(int userId, UpdateUserDTO user);
        public Task UpdateAvatarAsync(int userId, IFormFile newAvatar); 
        public Task<ImageDTO> GetUserAvatar(int userId);
    }
}
