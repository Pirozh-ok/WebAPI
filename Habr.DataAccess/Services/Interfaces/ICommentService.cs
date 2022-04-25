using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services.Interfaces
{
    public interface ICommentService
    {
        void CreateComment(int userId, int postId, string text);
        void DeleteComment(int commentId);
        void CreateCommentAnswer(int userId, string text, int parentId, int postId);
        IEnumerable<Comment> GetComments();
        IEnumerable<Comment> GetCommentsByUser(int userId);
        IEnumerable<Comment> GetCommentsByPost(int postId);
    }
}
