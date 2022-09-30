using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Guards
{
    public interface IPostGuard
    {
        void InvalidPost(string title, string text);
        void NotFoundUser(User? user);
        void NotFoundPost(Post? post);
        void EditNotPublishPost(Post updatePost); 
        void SendToDraftsPostWithComment(Post post);
        void AccessErrorEditPost(Post post, int userId);
        void PostAlreadePublished(Post post);
        void RateOutRange(int rate); 
        void PostNotPublished(Post post);
    }
}
