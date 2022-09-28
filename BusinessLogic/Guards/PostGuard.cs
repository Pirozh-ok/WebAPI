using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Guards
{
    public class PostGuard : IPostGuard
    {
        public void InvalidPost(string title, string text)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ValidationException(PostExceptionMessageResource.PostTitleRequired);
            }

            if (title.Length > 200)
            {
                throw new ValidationException(PostExceptionMessageResource.MaxLengthTitlePostExceeded);
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ValidationException(PostExceptionMessageResource.EmptyPostText);
            }

            if (text.Length > 2000)
            {
                throw new ValidationException(PostExceptionMessageResource.MaxLengthTextPostExceeded);
            }
        }

        public void NotFoundPost(Post? post)
        {
            if (post == null)
            {
                throw new NotFoundException(PostExceptionMessageResource.PostNotFound);
            }
        }

        public void NotFoundUser(User? user)
        {
            if (user == null)
            {
                throw new BadRequestException(UserExceptionMessageResource.UserNotFound);
            }
        }
    }
}
