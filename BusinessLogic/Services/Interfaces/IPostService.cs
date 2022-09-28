using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.Common.Parameters;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface IPostService
    {
        Task<PagedList<PostDTO>> GetFullPostsAsync(PostParameters postParameters);
        Task<Post> GetFullPostByIdAsync(int id);
        Task<PagedList<PostDTO>> GetPostsAsync(PostParameters postParameters);
        Task<PostDTO> GetPostByIdAsync(int id);
        Task<PagedList<PostDTO>> GetPostsByUserAsync(int userId, PostParameters postParameters);
        Task<PagedList<NotPublishedPostDTO>> GetNotPublishedPostsAsync(PostParameters postParameters);
        Task<NotPublishedPostDTO> GetNotPublishedPostByIdAsync(int id);
        Task<PagedList<NotPublishedPostDTO>> GetNotPublishedPostsByUserAsync(int userId, PostParameters postParameters);
        Task<PagedList<PublishedPostDTO>> GetPublishedPostsAsync(PostParameters postParameters);
        Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsAsyncV2(PostParameters postParameters);
        Task<PublishedPostDTO> GetPublishedPostByIdAsync(int postId);
        Task<PublishedPostDTOv2> GetPublishedPostByIdAsyncV2(int postId);
        Task<PagedList<PublishedPostDTO>> GetPublishedPostsByUserAsync(int userId, PostParameters postParameters);
        Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsByUserAsyncV2(int userId, PostParameters postParameters);
        Task CreatePostAsync(string title, string text, int userId, bool isPublished, List<IFormFile> images);
        Task UpdatePostAsync(Post post);
        Task PublishPostAsync(int postId);
        Task DeletePostAsync(int postId);
        Task SendPostToDraftsAsync(int postId);
        Task RatePost(int postId, int userId, int rate); 
        Task<List<PostRatingDTO>> GetRatingsByPostId(int postId);
    }
}
