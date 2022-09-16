using Habr.Common.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface ICommentService
    {
        Task CreateCommentAsync(int userId, int postId, string text);
        Task DeleteCommentAsync(int commentId);
        Task CreateCommentAnswerAsync(int userId, string text, int parentId, int postId);
        Task<CommentDTO> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<CommentDTO>> GetCommentsAsync();
        Task<IEnumerable<Comment>> GetFullCommentsAsync();
        Task<Comment> GetFullCommentByIdAsync(int id);
        Task<IEnumerable<CommentDTO>> GetCommentsByUserAsync(int userId);
        Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(int postId);
    }
}
