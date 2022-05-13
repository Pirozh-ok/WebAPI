using Habr.Common.DTOs;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync();
        Task<IEnumerable<PostDTO>> GetPostsDTOAsync();
        Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsDTOAsync(int userId);
        Task<IEnumerable<Post>> GetPublishedPostsAsync();
        Task<PublishedPostDTO> GetPublishedPostDTOAsync(int postId);
        Task<IEnumerable<Post>> GetPostsByUserAsync(int userId);
        Task<IEnumerable<Post>> GetPostsWithCommentAsync();
        Task<Post> GetPostByIdAsync(int id);
        void CreatePostAsync(string title, string text, int userId, bool isPublished);
        void UpdatePostAsync(Post post);
        void PublishPostAsync(int postId);
        void DeletePostAsync(int postId);
        Task<IEnumerable<Post>> GetPublishedPostsByUserAsync(int userId);
        Task<IEnumerable<Post>> GetNotPublishedPostsByUserAsync(int userId);
        void SendPostToDraftsAsync(int postId);
    }
}
