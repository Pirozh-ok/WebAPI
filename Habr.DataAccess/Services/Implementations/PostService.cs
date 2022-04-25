using Habr.DataAccess.Entities;
using Habr.DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Services
{
    public class PostService : IPostService
    {
        public void CreatePost(string title, string text, int userId)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new Exception("Пользователь  с таким id не найден!");

            context.Posts.Add(new Post() { Title = title, Text = text, User = user });
            context.SaveChanges();
        }

        public void DeletePost(int postId)
        {
            using var context = new DataContext();
            var deletePost = context.Posts.Where(p => p.Id == postId).SingleOrDefault();

            if (deletePost == null)
                throw new Exception("Пост с таким id не найден");

            context.Entry(deletePost)
                .Collection(c => c.Comments)
                .Load();

            context.Posts.Remove(deletePost);
            context.SaveChanges();
        }

        public Post GetPostById(int id)
        {
            using var context = new DataContext();
            var post = context.Posts.SingleOrDefault(p => p.Id == id);

            if (post == null)
                throw new Exception("Пост по заданому id не найден!");

            return post;
        }

        public IEnumerable<Post> GetPosts()
        {
            using var context = new DataContext();
            return context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            using var context = new DataContext();
            return context.Posts
                .Where(p => p.UserId == userId)
                .Include(u => u.User)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPostsWithComment()
        {
            using var context = new DataContext();
            return context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }

        public void UpdatePost(Post post)
        {
            using var context = new DataContext();
            var updatePost = context.Posts.SingleOrDefault(p => p.Id == post.Id);

            if (updatePost == null)
                throw new Exception("Пост не найден!");

            updatePost.User = context.Users.SingleOrDefault(u => u.Id == post.UserId);

            if (updatePost.User == null)
                throw new Exception("Пользователь, создавший пост, не найден!");

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;

            context.SaveChanges();
        }
    }
}
