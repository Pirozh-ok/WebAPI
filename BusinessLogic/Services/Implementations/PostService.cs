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
        private readonly IMapper _mapper;
        public PostService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            return _mapper.Map<List<NotPublishedPostDTO>>(posts);
        }

        public Post GetPostById(int postId)
        {
            var post = _context.Posts
                .Include(p => p.User)
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
            var posts = _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.Created);

            return _mapper.Map<List<PostDTO>>(posts);
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

        public PublishedPostDTO GetPublishedPostDTO(int postId)
        {
            var post = GetPostById(postId);

            if (!post.IsPublished)
            {
                throw new Exception("Пост не опубликован");
            }

            return new PublishedPostDTO
            {
                Title = post.Title,
                Text = post.Text,
                AuthorEmail = post.User.Email,
                PublicationDate = post.Updated,
                Comments = GetCommentsByPost(post.Id).ToList()
            };
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

        private IEnumerable<CommentDTO> GetCommentsByPost(int postId)
        {
            var postComments = _context.Comments
                .Where(p => p.PostId == postId && p.ParentId == null)
                .Include(p => p.User)
                .AsNoTracking()
                .ToList();

            var commentsDTO = new List<CommentDTO>();

            foreach (var comment in postComments)
            {
                commentsDTO.Add(
                    new CommentDTO
                    {
                        Text = comment.Text,
                        AuthorName = comment.User.Name,
                        Comments = GetCommentsByParentId(comment.Id)
                    });
            }

            return commentsDTO;
        }

        private IEnumerable<CommentDTO> GetCommentsByParentId(int parentId)
        {
            var subComments = _context.Comments
                .Include(c => c.SubComments)
                .Where(c => c.ParentId == parentId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToList();

            if (subComments is null || subComments.Count == 0)
            {
                return new List<CommentDTO>();
            }    

            var subCommentDTOs = new List<CommentDTO>();

            foreach (var subComment in subComments)
            {
                subCommentDTOs.Add(
                        new CommentDTO
                        {
                            Text = subComment.Text,
                            AuthorName = subComment.User.Name,
                            Comments = GetCommentsByParentId(subComment.Id)
                        });
            }

            return subCommentDTOs;
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
