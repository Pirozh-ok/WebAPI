using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _context;
        public CommentService(DataContext context)
        {
            _context = context;
        }

        public Comment GetCommentById(int commentId)
        {
            var comment = _context.Comments
                .SingleOrDefault(c => c.Id == commentId);

            GuardAgainstInvalidComment(comment);
            return comment;
        }
        public void CreateComment(int userId, int postId, string text)
        {
            var user = GetUserById(userId);
            var post = GetPostById(postId);

            _context.Comments.Add(
                new Comment
                {
                    UserId = userId,
                    PostId = postId,
                    Text = text
                });

            _context.SaveChanges();
        }

        public void CreateCommentAnswer(int userId, string text, int parentId, int postId)
        {
            var user = GetUserById(userId);
            var post = GetPostById(postId);

            var parent = _context.Comments
                .SingleOrDefault(c => c.Id == parentId);

            GuardAgainstInvalidComment(parent);

            _context.Comments.Add(new Comment
            {
                User = user,
                Text = text,
                Parent = parent,
                Post = post
            });

            _context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            var comment = GetCommentById(commentId);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }

        public IEnumerable<Comment> GetComments()
        {
            return _context.Comments
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Comment> GetCommentsByPost(int postId)
        {
            return _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Comment> GetCommentsByUser(int userId)
        {
            return _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }

        private User GetUserById(int userId)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Id == userId);

            GuardAgainstInvalidUser(user);
            return user;
        }

        private Post GetPostById(int postId)
        {
            var post = _context.Posts
                .SingleOrDefault(u => u.Id == postId);

            if (post == null)
            {
                throw new Exception("Пост с таким id не найден!");
            }

            return post;
        }

        private void GuardAgainstInvalidUser(User? user)
        {
            if (user == null)
            {
                throw new Exception("Пользователь не найден!");
            }
        }

        private void GuardAgainstInvalidComment(Comment comment)
        {
            if (comment == null)
            {
                throw new Exception("Комментарий не найден!");
            }
        }
    }
}
