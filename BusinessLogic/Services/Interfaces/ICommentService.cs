using Habr.Common.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Services.Interfaces
{
    public interface ICommentService
    {
        void CreateCommentAsync(int userId, int postId, string text);
        void DeleteCommentAsync(int commentId);
        void CreateCommentAnswerAsync(int userId, string text, int parentId, int postId);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<IEnumerable<CommentDTO>> GetCommentsByUserAsync(int userId);
        Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId);
    }
}
