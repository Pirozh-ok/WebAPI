using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetFullPostsAsync();
        Task<Post> GetFullPostByIdAsync(int id);
        Task<IEnumerable<PostDTO>> GetPostsAsync();
        Task<PostDTO> GetPostByIdAsync(int id);
        Task<IEnumerable<PostDTO>> GetPostsByUserAsync(int userId);
        Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsAsync();
        Task<NotPublishedPostDTO> GetNotPublishedPostByIdAsync(int id);
        Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsByUserAsync(int userId);
        Task<IEnumerable<PublishedPostDTO>> GetPublishedPostsAsync();
        Task<IEnumerable<PublishedPostDTOv2>> GetPublishedPostsAsyncV2();
        Task<PublishedPostDTO> GetPublishedPostByIdAsync(int postId);
        Task<PublishedPostDTOv2> GetPublishedPostByIdAsyncV2(int postId);
        Task<IEnumerable<PublishedPostDTO>> GetPublishedPostsByUserAsync(int userId);
        Task<IEnumerable<PublishedPostDTOv2>> GetPublishedPostsByUserAsyncV2(int userId);
        Task CreatePostAsync(string title, string text, int userId, bool isPublished);
        Task UpdatePostAsync(Post post);
        Task PublishPostAsync(int postId);
        Task DeletePostAsync(int postId);
        Task SendPostToDraftsAsync(int postId);
    }
}
