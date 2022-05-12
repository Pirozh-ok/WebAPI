using Habr.Common.DTOs;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<PostDTO> GetPostsDTO();
        IEnumerable<NotPublishedPostDTO> GetNotPublishedPostsDTO(int userId);
        IEnumerable<Post> GetPublishedPosts();
        PublishedPostDTO GetPublishedPostDTO(int postId);
        IEnumerable<Post> GetPostsByUser(int userId);
        IEnumerable<Post> GetPostsWithComment();
        Post GetPostById(int id);
        void CreatePost(string title, string text, int userId, bool isPublished);
        void UpdatePost(Post post);
        void PublishPost(int postId);
        void DeletePost(int postId);
        IEnumerable<Post> GetPublishedPostsByUser(int userId);
        IEnumerable<Post> GetNotPublishedPostsByUser(int userId);
        void SendPostToDrafts(int postId);
    }
}
