using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private User GetUserById(int userId)
        {
            using var context = new DataContext();
            var user = context.Users
                .SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("Пользователь  с таким id не найден!");
            }

            return user;
        }
        private Post GetPostById(int postId)
        {
            using var context = new DataContext();
            var post = context.Posts
                .SingleOrDefault(u => u.Id == postId);

            if (post == null)
            {
                throw new Exception("Пост с таким id не найден!");
            }

            return post;
        }

        public Comment GetCommentById(int commentId)
        {
            var context = new DataContext();
            var comment = context.Comments
                .SingleOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                throw new Exception("Комментарий не найден!");
            }

            return comment;
        }
        public void CreateComment(int userId, int postId, string text)
        {
            using var context = new DataContext();
            var user = GetUserById(userId);
            var post = GetPostById(postId);

            context.Comments.Add(new Comment { UserId = userId, PostId = postId, Text = text });
            context.SaveChanges();
        }

        public void CreateCommentAnswer(int userId, string text, int parentId, int postId)
        {

            using var context = new DataContext();
            var user = GetUserById(userId);
            var post = GetPostById(postId);

            var parent = context.Comments
                .SingleOrDefault(c => c.Id == parentId);

            if (parent == null)
            {
                throw new Exception("Комментарий, к которому написан ответ, не найден!");
            }

            context.Comments.Add(new Comment
            {
                User = user,
                Text = text,
                Parent = parent,
                Post = post
            });

            context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            using var context = new DataContext();
            var comment = GetCommentById(commentId);

            context.Comments.Remove(comment);
            context.SaveChanges();
        }

        public IEnumerable<Comment> GetComments()
        {
            using var context = new DataContext();
            return context.Comments
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Comment> GetCommentsByPost(int postId)
        {
            using var context = new DataContext();
            return context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Comment> GetCommentsByUser(int userId)
        {
            using var context = new DataContext();
            return context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }
    }
}
