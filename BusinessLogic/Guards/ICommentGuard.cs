using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Guards
{
    public interface ICommentGuard
    {
        void NotFoundComment(Comment? comment);
        void AccessError(int userId, Comment comment);
        void InvalidListComment<T>(IEnumerable<T> comments); 
        void NotFoundUser(User? user);
        void NotFoundPost(Post? post);
    }
}
