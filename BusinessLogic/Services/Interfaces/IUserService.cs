using Habr.Common.DTOs;
using Habr.Common.DTOs.ImageDTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityDTO> SignInAsync(UserSignInDTO userSignInData);
        Task<IdentityDTO> SignUpAsync(CreateUserDTO newUserData);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<IEnumerable<UserWithPostsDTO>> GetUsersWithPostsAsync();
        Task<IEnumerable<UserWithCommentsDTO>> GetUsersWithCommentsAsync();
        Task DeleteAsync(int id);
        Task UpdateAsync(int userId, UpdateUserDTO user);
        Task UpdateAvatarAsync(int userId, IFormFile newAvatar); 
        Task<ImageDTO> GetUserAvatar(int userId);
        Task SubscribeToUser(int fromUserId, int toUserId);
        Task UnsubscribeFromUser(int fromUserId, int toUserId);
        Task<IEnumerable<UserSubscriptionDTO>> GetSubscriptions(int userId);
        Task<IEnumerable<PublishedPostDTO>> GetNews(int userId); 
    }
}
