using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.Exceptions;
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
        public async Task CreatePostAsync(string title, string text, int userId, bool isPublished)
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
        public async Task DeletePostAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);

            await _context.Entry(post)
                .Collection(c => c.Comments)
                .LoadAsync();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Post>> GetFullPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Post> GetFullPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == id);

            GuardAgainstInvalidPost(post);

            return post;
        }
        public async Task<IEnumerable<PostDTO>> GetPostsAsync()
        {
            return await _context.Posts
                .Include(u => u.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<PostDTO> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstInvalidPost(post);
            return _mapper.Map<PostDTO>(post);
        }
        public async Task<IEnumerable<PostDTO>> GetPostsByUserAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(u => u.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<NotPublishedPostDTO> GetNotPublishedPostByIdAsync(int id)
        {
            var post = await GetFullPostByIdAsync(id);

            if(post.IsPublished)
            {
                throw new BusinessException(Common.Resources.PostExceptionMessageResource.PostAlreadyPublished);
            }

            return _mapper.Map<NotPublishedPostDTO>(post);
        }
        public async Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsByUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return await _context.Posts
                .Where(p => p.UserId == userId && !p.IsPublished)
                .ProjectTo<NotPublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<NotPublishedPostDTO>> GetNotPublishedPostsAsync()
        {
            return await _context.Posts
                .Where(p => !p.IsPublished)
                .ProjectTo<NotPublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .OrderByDescending(p => p.Updated)
                .ToListAsync();
        }
        public async Task<IEnumerable<PublishedPostDTO>> GetPublishedPostsAsync()
        {
            var posts = await _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            foreach(var post in posts)
            {
                post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            }

            return posts; 
        }
        public async Task<PublishedPostDTO> GetPublishedPostByIdAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);

            if (!post.IsPublished)
            {
                throw new BusinessException(Common.Resources.PostExceptionMessageResource.PostNotPublished);
            }

            var postDTO = _mapper.Map<PublishedPostDTO>(post);
            postDTO.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            return postDTO;
        }
        public async Task<IEnumerable<PublishedPostDTO>> GetPublishedPostsByUserAsync(int userId)
        {
            var posts = await _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished && p.UserId == userId)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            foreach (var post in posts)
            {
                post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            }

            return posts;
        }
        public async Task PublishPostAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);

            if (post.IsPublished)
            {
                throw new BusinessException("The post has already been published!");
            }

            post.User = await GetUserByIdAsync(post.UserId);
            post.IsPublished = true;
            var modifiedPost = _context.Entry(post);
            modifiedPost.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task SendPostToDraftsAsync(int postId)
        { 
            var post = await _context.Posts
                .Include(p => p.Comments)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstInvalidPost(post);

            if (post.Comments.Count > 0)
            {
                throw new BusinessException("A post with comments cannot be sent to drafts!");
            }

            post.IsPublished = false;
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePostAsync(Post post)
        {
            var updatePost = await GetFullPostByIdAsync(post.Id);

            if (updatePost.IsPublished)
            {
                throw new BusinessException(Common.Resources.PostExceptionMessageResource.PostCannotBeEdited);
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
                throw new NotFoundException(Common.Resources.PostExceptionMessageResource.PostNotFound);
            }
        }
        private void GuardAgainstInvalidPost(string title, string text)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ValidationException(Common.Resources.PostExceptionMessageResource.PostTitleRequired);
            }

            if (title.Length > 200)
            {
                throw new ValidationException(Common.Resources.PostExceptionMessageResource.MaxLengthTitlePostExceeded);
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ValidationException(Common.Resources.PostExceptionMessageResource.EmptyPostText);
            }

            if (title.Length > 2000)
            {
                throw new ValidationException(Common.Resources.PostExceptionMessageResource.MaxLengthTextPostExceeded);
            }
        }
        private void GuardAgainstInvalidUser (User? user)
        {
            if (user == null)
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.UserNotFound);
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
