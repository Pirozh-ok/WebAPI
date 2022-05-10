using AutoMapper;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        public PostService(DataContext context)
        {
            _context = context;
        }
        public void CreatePost(string title, string text, int userId, bool isPublished)
        {
            GuardAgainstInvalidPost(title, text);
            var user = GetUserById(userId);

            _context.Posts.Add(
                new Post()
                {
                    Title = title,
                    Text = text,
                    User = user,
                    IsPublished = isPublished
                });
            _context.SaveChanges();
        }

        public void DeletePost(int postId)
        {
            var post = GetPostById(postId);

            _context.Entry(post)
                .Collection(c => c.Comments)
                .Load();

            _context.Posts.Remove(post);
            _context.SaveChanges();
        }

        public IEnumerable<Post> GetNotPublishedPostsByUser(int userId)
        {
            var user = GetUserById(userId);
            return _context.Posts.Where(p => p.UserId == userId && !p.IsPublished).ToList();
        }

        public IEnumerable<NotPublishedPostDTO> GetNotPublishedPostsDTO(int userId)
        {
            var posts = GetNotPublishedPostsByUser(userId)
                .OrderByDescending(p => p.Updated);

            var config = new MapperConfiguration(x => x.CreateMap<Post, NotPublishedPostDTO>()
                .ForMember("Title", c => c.MapFrom(x => x.Title))
                .ForMember("Created", c => c.MapFrom(x => x.Created))
                .ForMember("Updated", c => c.MapFrom(x => x.User.Email)));

            var mapper = new Mapper(config);
            return mapper.Map<List<NotPublishedPostDTO>>(posts);
        }

        public Post GetPostById(int postId)
        {
            var post = _context.Posts
                .SingleOrDefault(p => p.Id == postId);

            GuardAgainstInvalidPost(post);
            return post;
        }

        public IEnumerable<Post> GetPosts()
        {
            return _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            return _context.Posts
                .Where(p => p.UserId == userId)
                .Include(u => u.User)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<PostDTO> GetPostsDTO()
        {
            var config = new MapperConfiguration(x => x.CreateMap<Post, PostDTO>()
                .ForMember("Title", c => c.MapFrom(x => x.Title))
                .ForMember("EmailAuthor", c => c.MapFrom(x => x.User.Email))
                .ForMember("CreateDate", c => c.MapFrom(x => x.Created)));

            var mapper = new Mapper(config);
            var posts = _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.Created);

            return mapper.Map<List<PostDTO>>(posts);
        }

        public IEnumerable<Post> GetPostsWithComment()
        {
            return _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.SubComments)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPublishedPosts()
        {
            return _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ToList();
        }

        public IEnumerable<Post> GetPublishedPostsByUser(int userId)
        {
            var user = GetUserById(userId);
            return _context.Posts.Where(p => p.UserId == userId && p.IsPublished).ToList();
        }

        public void PublishPost(int postId)
        {
            var post = GetPostById(postId);

            if (post.IsPublished)
            {
                throw new Exception("Пост уже опубликован!");
            }

            post.User = GetUserById(post.UserId);
            post.IsPublished = true;
            var modifiedPost = _context.Entry(post);
            modifiedPost.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void SendPostToDrafts(int postId)
        { 
            var post = _context.Posts
                .Include(p => p.Comments)
                .SingleOrDefault(p => p.Id == postId);

            GuardAgainstInvalidPost(post);

            if (post.Comments.Count > 0)
            {
                throw new Exception("Пост с комментариями нельзя отправить в черновики!");
            }

            post.IsPublished = false;
            _context.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            var updatePost = GetPostById(post.Id);

            if (updatePost.IsPublished)
            {
                throw new Exception("Опубликованный пост нельзя редактировать!");
            }

            updatePost.User = _context.Users
                .SingleOrDefault(u => u.Id == post.UserId);

            GuardAgainstInvalidUser(updatePost.User);

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;
            _context.SaveChanges();
        }

        private void GuardAgainstInvalidPost(Post? post)
        {
            if (post == null)
            {
                throw new Exception("Пост не найден!");
            }
        }
        private void GuardAgainstInvalidPost(string title, string text)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new Exception("Заголовок поста является обязательным!");
            }

            if (title.Length > 200)
            {
                throw new Exception("Заголовок поста не может превышать 200 символов!");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Текст поста является обязательным!");
            }

            if (title.Length > 2000)
            {
                throw new Exception("Текст поста не может превышать 2000 символов!");
            }
        }

        private void GuardAgainstInvalidUser (User? user)
        {
            if (user == null)
            {
                throw new Exception("Пользователь не найден!");
            }
        }
        private User GetUserById(int userId)
        {
            
            var user = _context.Users
                .SingleOrDefault(u => u.Id == userId);

            GuardAgainstInvalidUser(user);
            return user;
        }
    }
}
