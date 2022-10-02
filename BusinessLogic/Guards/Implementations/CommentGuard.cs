using Habr.BusinessLogic.Guards.Interfaces;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Guards.Implementations
{
    public class CommentGuard : ICommentGuard
    {
        public void NotFoundComment(Comment? comment)
        {
            if (comment is null)
            {
                throw new NotFoundException(CommentExceptionMessageResource.CommentNotFound);
            }
        }

        public void AccessError(int userId, Comment comment)
        {
            if (userId != comment.UserId && comment.User.Role != Roles.Admin)
            {
                throw new AuthorizationException(CommentExceptionMessageResource.AccessError);
            }
        }

        public void InvalidListComment<T>(IEnumerable<T> comments)
        {
            if (comments is null || comments.Count() == 0)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }

        public void NotFoundUser(User? user)
        {
            if (user is null)
            {
                throw new NotFoundException(UserExceptionMessageResource.UserNotFound);
            }
        }

        public void NotFoundPost(Post? post)
        {
            if (post is null)
            {
                throw new NotFoundException(PostExceptionMessageResource.PostNotFound);
            }
        }
    }
}
