using AutoMapper;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class PostService : IPostService
    {
        public void CreatePost(string title, string text, int userId, bool isPublished)
        {
            if (string.IsNullOrEmpty( title ))
            {
                throw new Exception( "Заголовок поста является обязательным!" );
            }

            if (title.Length > 200)
            {
                throw new Exception( "Заголовок поста не может превышать 200 символов!" );
            }

            if (string.IsNullOrEmpty( text ))
            {
                throw new Exception( "Текст поста является обязательным!" );
            }

            if (title.Length > 2000)
            {
                throw new Exception( "Текст поста не может превышать 2000 символов!" );
            }

            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Id == userId );

            if (user == null)
            {
                throw new Exception( "Пользователь  с таким id не найден!" );
            }

            context.Posts.Add( new Post() { Title = title, Text = text, User = user, IsPublished = isPublished } );
            context.SaveChanges();
        }

        public void DeletePost(int postId)
        {
            using var context = new DataContext();
            var deletePost = GetPostById( postId );

            context.Entry( deletePost )
                .Collection( c => c.Comments )
                .Load();

            context.Posts.Remove( deletePost );
            context.SaveChanges();
        }

        public IEnumerable<Post> GetNotPublishedPostsByUser(int userId)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Id == userId );

            if (user == null)
            {
                throw new Exception( "Пользователь не найден!" );
            }

            return context.Posts.Where( p => p.UserId == userId && !p.IsPublished ).ToList();
        }

        public IEnumerable<NotPublishedPostDTO> GetNotPublishedPostsDTO(int userId)
        {
            var posts = GetNotPublishedPostsByUser( userId ).OrderByDescending(p => p.Updated);

            var config = new MapperConfiguration( x => x.CreateMap<Post, NotPublishedPostDTO>()
            .ForMember( "Title", c => c.MapFrom( x => x.Title ) )
            .ForMember( "Created", c => c.MapFrom( x => x.Created ) )
            .ForMember( "Updated", c => c.MapFrom( x => x.User.Email ) ) );

            var mapper = new Mapper( config );
            return mapper.Map<List<NotPublishedPostDTO>>( posts );
        }

        public Post GetPostById(int postId)
        {
            using var context = new DataContext();
            var post = context.Posts.SingleOrDefault( p => p.Id == postId );

            if (post == null)
            {
                throw new Exception( "Пост не найден!" );
            }

            return post;
        }

        public IEnumerable<Post> GetPosts()
        {
            using var context = new DataContext();
            return context.Posts
                .Include( u => u.User )
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            using var context = new DataContext();
            return context.Posts
                .Where( p => p.UserId == userId )
                .Include( u => u.User )
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<PostDTO> GetPostsDTO()
        {
            using var context = new DataContext();
            var config = new MapperConfiguration( x => x.CreateMap<Post, PostDTO>()
            .ForMember( "Title", c => c.MapFrom( x => x.Title ) )
            .ForMember( "EmailAuthor", c => c.MapFrom( x => x.User.Email ) )
            .ForMember( "CreateDate", c => c.MapFrom( x => x.Created ) ) );

            var mapper = new Mapper( config );
            var posts = context.Posts
                .Include( p => p.User )
                .OrderByDescending( p => p.Created );

            return mapper.Map<List<PostDTO>>( posts );
        }

        public IEnumerable<Post> GetPostsWithComment()
        {
            using var context = new DataContext();
            return context.Posts
                .Include( p => p.Comments )
                .ThenInclude( c => c.SubComments )
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Post> GetPublishedPosts()
        {
            using var context = new DataContext();
            return context.Posts
                .Include( u => u.User )
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ToList();
        }

        public IEnumerable<Post> GetPublishedPostsByUser(int userId)
        {
            using var context = new DataContext();
            var user = context.Users.SingleOrDefault( u => u.Id == userId );

            if (user == null)
            {
                throw new Exception( "Пользователь  с таким id не найден!" );
            }

            return context.Posts.Where( p => p.UserId == userId && p.IsPublished ).ToList();
        }

        public void PublishPost(int postId)
        {
            using var context = new DataContext();
            var post = GetPostById( postId );

            if (post.IsPublished)
            {
                throw new Exception( "Пост уже опубликован!" );
            }

            post.User = context.Users.SingleOrDefault( u => u.Id == post.UserId );

            if (post.User == null)
            {
                throw new Exception( "Пользователь, создавший пост, не найден!" );
            }

            post.IsPublished = true;
            context.Entry(post).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void SendPostToDrafts(int postId)
        {
            using var context = new DataContext();
            var post = context.Posts.Include(p => p.Comments).SingleOrDefault(p => p.Id == postId);

            if(post == null)
            {
                throw new Exception( "Пост не найден!" );
            }

            if(post.Comments.Count > 0)
            {
                throw new Exception("Пост с комментариями нельзя отправить в черновики!");
            }

            post.IsPublished = false;
            context.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            using var context = new DataContext();
            var updatePost = GetPostById( post.Id );

            if (updatePost.IsPublished)
            {
                throw new Exception( "Опубликованный пост нельзя редактировать!" );
            }

            updatePost.User = context.Users.SingleOrDefault( u => u.Id == post.UserId );

            if (updatePost.User == null)
            {
                throw new Exception( "Пользователь не найден!" );
            }

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;
            context.SaveChanges();
        }
    }
}
