using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Extensions;
using Habr.Common.Parameters;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PostService(DataContext context, IMapper mapper, ILogger<PostService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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
            
            if(isPublished)
            {
                _logger.LogInformation($"\"{title}\" {LogResources.PublishPost}");
            }
        }

        public async Task DeletePostAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);
            GuardAgainstPostNotFound(post);

            await _context.Entry(post)
                .Collection(c => c.Comments)
                .LoadAsync();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<PostDTO>> GetFullPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<Post> GetFullPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == id);

            GuardAgainstPostNotFound(post);

            return post;
        }

        public async Task<PagedList<PostDTO>> GetPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(u => u.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<PostDTO> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstPostNotFound(post);
            return _mapper.Map<PostDTO>(post);
        }

        public async Task<PagedList<PostDTO>> GetPostsByUserAsync(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.UserId == userId)
                .Include(u => u.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<NotPublishedPostDTO> GetNotPublishedPostByIdAsync(int id)
        {
            var post = await GetFullPostByIdAsync(id);

            if(post.IsPublished)
            {
                throw new BusinessException(PostExceptionMessageResource.PostAlreadyPublished);
            }

            return _mapper.Map<NotPublishedPostDTO>(post);
        }

        public async Task<PagedList<NotPublishedPostDTO>> GetNotPublishedPostsByUserAsync(int userId, PostParameters postParameters)
        {
            var user = await GetUserByIdAsync(userId);
            GuardAgainstInvalidUser(user); 

            var posts = _context.Posts
                .Where(p => p.UserId == userId && !p.IsPublished)
                .ProjectTo<NotPublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<PagedList<NotPublishedPostDTO>> GetNotPublishedPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => !p.IsPublished)
                .ProjectTo<NotPublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .OrderByDescending(p => p.Updated);

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<PagedList<PublishedPostDTO>> GetPublishedPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider);

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            if (posts is not null)
            {
                foreach (var post in posts)
                {
                    post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
                }
            }

            return pagedPosts; 
        }

        public async Task<PublishedPostDTO> GetPublishedPostByIdAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);

            if (!post.IsPublished)
            {
                throw new BusinessException(PostExceptionMessageResource.PostNotPublished);
            }

            var postDTO = _mapper.Map<PublishedPostDTO>(post);
            postDTO.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            return postDTO;
        }

        public async Task<PagedList<PublishedPostDTO>> GetPublishedPostsByUserAsync(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished && p.UserId == userId)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider);

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            if (posts is not null)
            {
                foreach (var post in posts)
                {
                    post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
                }
            }

            return pagedPosts;
        }

        public async Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsAsyncV2(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .ProjectTo<PublishedPostDTOv2>(_mapper.ConfigurationProvider);

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            if (posts is not null)
            {
                foreach (var post in posts)
                {
                    post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
                }
            }

            return pagedPosts;
        }

        public async Task<PublishedPostDTOv2> GetPublishedPostByIdAsyncV2(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);

            if (!post.IsPublished)
            {
                throw new BusinessException(PostExceptionMessageResource.PostNotPublished);
            }

            var postDTO = _mapper.Map<PublishedPostDTOv2>(post);
            postDTO.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            return postDTO;
        }

        public async Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsByUserAsyncV2(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Include(u => u.User)
                .AsNoTracking()
                .Where(p => p.IsPublished && p.UserId == userId)
                .ProjectTo<PublishedPostDTOv2>(_mapper.ConfigurationProvider);

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            if (posts is not null)
            {
                foreach (var post in posts)
                {
                    post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
                }
            }

            return pagedPosts;
        }

        public async Task PublishPostAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);
            GuardAgainstPostNotFound(post);

            if (post.IsPublished)
            {
                throw new BusinessException(PostExceptionMessageResource.PostAlreadyPublished);
            }

            post.User = await GetUserByIdAsync(post.UserId);
            post.IsPublished = true;
            var modifiedPost = _context.Entry(post);
            modifiedPost.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\"{post.Title}\" {LogResources.PublishPost}");
        }

        public async Task SendPostToDraftsAsync(int postId)
        { 
            var post = await _context.Posts
                .Include(p => p.Comments)
                .SingleOrDefaultAsync(p => p.Id == postId);

            GuardAgainstPostNotFound(post);

            if (post.Comments.Count > 0)
            {
                throw new BusinessException(PostExceptionMessageResource.CannotSendDrafts);
            }

            post.IsPublished = false;
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            var updatePost = await GetFullPostByIdAsync(post.Id);

            if (updatePost.IsPublished)
            {
                throw new BusinessException(PostExceptionMessageResource.PostCannotBeEdited);
            }

            updatePost.User = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == post.UserId);

            GuardAgainstInvalidUser(updatePost.User);

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;
            await _context.SaveChangesAsync();
        }

        public async Task RatePost(int postId, int userId, int rate)
        {
            if (rate < 1 || rate > 5)
            {
                throw new BusinessException(PostExceptionMessageResource.RateExceedsLimits);
            }

            var post = await GetFullPostByIdAsync(postId);
            GuardAgainstPostNotFound(post);

            if (!post.IsPublished)
                return;

            var user = await GetUserByIdAsync(userId);
            GuardAgainstInvalidUser(user);

            var postRating = await _context.PostsRatings
                .SingleOrDefaultAsync(r => r.UserId == userId && r.PostId == postId);

            if (postRating is null)
            {
                _context.PostsRatings.Add(
                    new PostRating
                    {
                        User = user,
                        Post = post,
                        Value = rate,
                        DateLastModified = DateTime.UtcNow,
                    });
            }
            else
            {
                postRating.Value = rate;
            }

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

        private void GuardAgainstPostNotFound(Post? post)
        {
            if (post == null)
            {
                throw new NotFoundException(PostExceptionMessageResource.PostNotFound);
            }
        }

        private void GuardAgainstInvalidPost(string title, string text)
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

        private void GuardAgainstInvalidUser (User? user)
        {
            if (user == null)
            {
                throw new BadRequestException(UserExceptionMessageResource.UserNotFound);
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
