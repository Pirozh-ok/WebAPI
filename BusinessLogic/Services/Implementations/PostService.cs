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
        public async void CreatePostAsync(string title, string text, int userId, bool isPublished)
        {
            GuardAgainstInvalidPost(title, text);
            var user = await GetUserByIdAsync(userId);

            await _context.Posts.AddAsync(
                new Post()
                {
                    Title = title,
                    Text = text,
                    User = user,
                    IsPublished = isPublished
                });
            await _context.SaveChangesAsync();
        }

        public async void DeletePostAsync(int postId)
        {
            var post = await GetPostByIdAsync(postId);

            await _context.Entry(post)
                .Collection(c => c.Comments)
                .LoadAsync();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Post>> GetNotPublishedPostsByUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return await _context.Posts
                .Where(p => p.UserId == userId && !p.IsPublished)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsDTOAsync(int userId)
        {
            var posts = await GetNotPublishedPostsByUserAsync(userId);

            return _mapper.Map<List<NotPublishedPostDTO>>(posts
                .OrderByDescending(p => p.Updated)
                .ToList());
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstInvalidPost(post);
            return post;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(u => u.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<PostDTO>> GetPostsDTOAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.Created)
                .ToListAsync();

            return _mapper.Map<List<PostDTO>>(posts);
        }

        public async Task<IEnumerable<Post>> GetPostsWithCommentAsync()
        {
            return await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.SubComments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            return await _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ToListAsync();
        }

        public async Task<PublishedPostDTO> GetPublishedPostDTOAsync(int postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (!post.IsPublished)
            {
                throw new Exception("Post not published");
            }

            var comments = await GetCommentsByPostAsync(post.Id);

            return new PublishedPostDTO
            {
                Title = post.Title,
                Text = post.Text,
                AuthorEmail = post.User.Email,
                PublicationDate = post.Updated,
                Comments = comments.ToList()
            };
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsByUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return await _context
                .Posts
                .Where(p => p.UserId == userId && p.IsPublished)
                .ToListAsync();
        }

        public async void PublishPostAsync(int postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (post.IsPublished)
            {
                throw new Exception("The post has already been published!");
            }

            post.User = await GetUserByIdAsync(post.UserId);
            post.IsPublished = true;
            var modifiedPost = _context.Entry(post);
            modifiedPost.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async void SendPostToDraftsAsync(int postId)
        { 
            var post = await _context.Posts
                .Include(p => p.Comments)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstInvalidPost(post);

            if (post.Comments.Count > 0)
            {
                throw new Exception("A post with comments cannot be sent to drafts!");
            }

            post.IsPublished = false;
            await _context.SaveChangesAsync();
        }

        public async void UpdatePostAsync(Post post)
        {
            var updatePost = await GetPostByIdAsync(post.Id);

            if (updatePost.IsPublished)
            {
                throw new Exception("Published post cannot be edited!");
            }

            updatePost.User = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == post.UserId);

            GuardAgainstInvalidUser(updatePost.User);

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;
            await _context.SaveChangesAsync();
        }

        private async Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(int postId)
        {
            var postComments = await _context.Comments
                .Where(p => p.PostId == postId && p.ParentId == null)
                .Include(p => p.User)
                .AsNoTracking()
                .ToListAsync();

            var commentsDTO = new List<CommentDTO>();

            foreach (var comment in postComments)
            {
                commentsDTO.Add(
                    new CommentDTO
                    {
                        Text = comment.Text,
                        AuthorName = comment.User.Name,
                        Comments = await GetCommentsByParentIdAsync(comment.Id)
                    });
            }

            return commentsDTO;
        }

        private async Task<IEnumerable<CommentDTO>> GetCommentsByParentIdAsync(int parentId)
        {
            var subComments = await _context.Comments
                .Include(c => c.SubComments)
                .Where(c => c.ParentId == parentId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();

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
                            Comments = await GetCommentsByParentIdAsync(subComment.Id)
                        });
            }

            return subCommentDTOs;
        }
        private void GuardAgainstInvalidPost(Post? post)
        {
            if (post == null)
            {
                throw new Exception("Post not found!");
            }
        }
        private void GuardAgainstInvalidPost(string title, string text)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new Exception("The post title is required!");
            }

            if (title.Length > 200)
            {
                throw new Exception("Post title cannot exceed 200 characters!");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Post text is required!");
            }

            if (title.Length > 2000)
            {
                throw new Exception("Post text cannot exceed 2000 characters!");
            }
        }

        private void GuardAgainstInvalidUser (User? user)
        {
            if (user == null)
            {
                throw new Exception("User is not found!");
            }
        }
        private async Task<User> GetUserByIdAsync(int userId)
        {
            
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            GuardAgainstInvalidUser(user);
            return user;
        }
    }
}
