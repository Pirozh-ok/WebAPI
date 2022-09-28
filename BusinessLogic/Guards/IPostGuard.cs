using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Guards
{
    public interface IPostGuard
    {
        void InvalidPost(string title, string text);
        void NotFoundUser(User? user);
        void NotFoundPost(Post? post);
    }
}
