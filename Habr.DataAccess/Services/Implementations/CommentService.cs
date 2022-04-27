using Habr.DataAccess.Entities;
using Habr.DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Services
{
    public class CommentService : ICommentService
    {
        public void CreateComment(int userId, int postId, string text)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Id == userId );

            if (user == null)
            {
                throw new Exception( "Пользователь, написавший комментарий, не найден!" );
            }

            var post = context.Posts.SingleOrDefault( p => p.Id == postId );

            if (post == null)
            {
                throw new Exception( "Пост, к которому написан комментарий, не найден!" );
            }

            context.Comments.Add( new Comment { UserId = userId, PostId = postId, Text = text } );
            context.SaveChanges();
        }

        public void CreateCommentAnswer(int userId, string text, int parentId, int postId)
        {

            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Id == userId );

            if (user == null)
            {
                throw new Exception( "Пользователь, написавший комментарий, не найден!" );
            }

            var post = context.Posts.SingleOrDefault( p => p.Id == postId );

            if (post == null)
            {
                throw new Exception( "Пост, к которому написан комментарий, не найден!" );
            }

            var parent = context.Comments.SingleOrDefault( c => c.Id == parentId );

            if (parent == null)
            {
                throw new Exception( "Комментарий, к которому написан ответ, не найден!" );
            }

            context.Comments.Add( new Comment
            {
                User = user,
                Text = text,
                Parent = parent,
                Post = post
            } );

            context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            using var context = new DataContext();
            var deleteComment = context.Comments.SingleOrDefault( c => c.Id == commentId );

            if (deleteComment == null)
            {
                throw new Exception( "Комментарий для удаления не найден!" );
            }

            context.Comments.Remove( deleteComment );
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
                .Where( c => c.PostId == postId )
                .Include( c => c.User )
                .Include( c => c.SubComments )
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Comment> GetCommentsByUser(int userId)
        {
            using var context = new DataContext();
            return context.Comments
                .Where( c => c.UserId == userId )
                .Include( c => c.SubComments )
                .AsNoTracking()
                .ToList();
        }
    }
}
