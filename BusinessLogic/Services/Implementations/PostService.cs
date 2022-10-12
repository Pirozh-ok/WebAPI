using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Guards.Interfaces;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common;
using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.Common.Extensions;
using Habr.Common.Parameters;
using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly IPostGuard _guard; 

        public PostService(
            DataContext context, 
            IMapper mapper, 
            ILogger<PostService> logger, 
            IFileManager fileManager,
            IPostGuard guard)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileManager = fileManager; 
            _guard = guard;
        }

        public async Task CreatePostAsync(string title, string text, int userId, bool isPublished, List<IFormFile> images)
        {
            _guard.InvalidPost(title, text);

            var user = await GetUserByIdAsync(userId);
            var post = new Post()
            {
                Title = title,
                Text = text,
                User = user,
                IsPublished = isPublished
            };

            if (images?.Count > 0)
            {
                foreach (var image in images)
                {
                    post.Images.Add(
                        new PostImage
                        {
                            PathImage = await _fileManager.SaveFile(image, user.Id)
                        });
                }
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            if (isPublished)
            {
                _logger.LogInformation($"\"{title}\" {LogResources.PublishPost}");
            }
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            var post = await GetFullPostByIdAsync(postId);
            _guard.NotFoundPost(post);
            _guard.AccessErrorEditPost(post, userId);

            await _context.Entry(post)
                .Collection(c => c.Comments)
                .LoadAsync();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<PostDTO>> GetFullPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<Post> GetFullPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == id);

            _guard.NotFoundPost(post);

            return post;
        }

        public async Task<PagedList<PostDTO>> GetPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<PostDTO> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == postId);

            _guard.NotFoundPost(post);
            return _mapper.Map<PostDTO>(post);
        }

        public async Task<PagedList<PostDTO>> GetPostsByUserAsync(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.UserId == userId)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<NotPublishedPostDTO> GetNotPublishedPostByIdAsync(int id)
        {
            var post = await GetFullPostByIdAsync(id);
            _guard.PostAlreadePublished(post);
            return _mapper.Map<NotPublishedPostDTO>(post);
        }

        public async Task<PagedList<NotPublishedPostDTO>> GetNotPublishedPostsByUserAsync(int userId, PostParameters postParameters)
        {
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
                .OrderByDescending(p => p.Updated)
                .AsNoTracking();

            return await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);
        }

        public async Task<PagedList<PublishedPostDTO>> GetPublishedPostsAsync(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.IsPublished)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            //if (posts is not null)
            //{
            //    foreach (var post in posts)
            //    {
            //        post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            //    }
            //}

            return pagedPosts; 
        }

        public async Task<PublishedPostDTO> GetPublishedPostByIdAsync(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);
            _guard.PostNotPublished(post);
           
            var postDTO = _mapper.Map<PublishedPostDTO>(post);
            //postDTO.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            return postDTO;
        }

        public async Task<PagedList<PublishedPostDTO>> GetPublishedPostsByUserAsync(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.IsPublished && p.UserId == userId)
                .ProjectTo<PublishedPostDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            //if (posts is not null)
            //{
            //    foreach (var post in posts)
            //    {
            //        post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            //    }
            //}

            return pagedPosts;
        }

        public async Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsAsyncV2(PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.IsPublished)
                .ProjectTo<PublishedPostDTOv2>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            //if (posts is not null)
            //{
            //    foreach (var post in posts)
            //    {
            //        post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            //    }
            //}

            return pagedPosts;
        }

        public async Task<PublishedPostDTOv2> GetPublishedPostByIdAsyncV2(int postId)
        {
            var post = await GetFullPostByIdAsync(postId);
            _guard.PostNotPublished(post);

            var postDTO = _mapper.Map<PublishedPostDTOv2>(post);
            //postDTO.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            return postDTO;
        }

        public async Task<PagedList<PublishedPostDTOv2>> GetPublishedPostsByUserAsyncV2(int userId, PostParameters postParameters)
        {
            var posts = _context.Posts
                .Where(p => p.IsPublished && p.UserId == userId)
                .ProjectTo<PublishedPostDTOv2>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var pagedPosts = await posts.ToPagedListAsync(postParameters.PageNumber, postParameters.PageSize);

            //if (posts is not null)
            //{
            //    foreach (var post in posts)
            //    {
            //        post.Comments = (await GetCommentsByPostAsync(post.Id)).ToList();
            //    }
            //}

            return pagedPosts;
        }

        public async Task PublishPostAsync(int postId, int userId)
        {
            var post = await GetFullPostByIdAsync(postId);
            _guard.NotFoundPost(post);
            _guard.AccessErrorEditPost(post, userId); 
            _guard.PostAlreadePublished(post);
            
            post.IsPublished = true;
            post.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\"{post.Title}\" {LogResources.PublishPost}");
        }

        public async Task SendPostToDraftsAsync(int userId, int postId)
        { 
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .SingleOrDefaultAsync(p => p.Id == postId);

            _guard.NotFoundPost(post);          
            _guard.AccessErrorEditPost(post!, userId);
            _guard.SendToDraftsPostWithComment(post!);

            post!.IsPublished = false;
            post.Updated = DateTime.UtcNow; 
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(int userId, UpdatePostDTO post)
        {
            var updatePost = await GetFullPostByIdAsync(post.PostId);
            _guard.NotFoundPost(updatePost);
            _guard.AccessErrorEditPost(updatePost, userId);
            _guard.EditNotPublishPost(updatePost);          

            updatePost.Title = post.Title;
            updatePost.Text = post.Text;
            updatePost.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task RatePost(int postId, int userId, int rate)
        {
            _guard.RateOutRange(rate); 

            var post = await _context.Posts
                .Include(p => p.PostsRatings)
                .SingleOrDefaultAsync(p => p.Id == postId);

            _guard.NotFoundPost(post);

            if (!post!.IsPublished)
            {
                return;
            }

            var postRating = post.PostsRatings
                .SingleOrDefault(r => r.UserId == userId); 

            if (postRating is null)
            {
                _context.PostsRatings.Add(
                    new PostRating
                    {
                        UserId = userId,
                        PostId = postId,
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

        public async Task<List<PostRatingDTO>> GetRatingsByPostId(int postId)
        {
            return  await _context.PostsRatings
                .Where(pr => pr.PostId == postId)
                .ProjectTo<PostRatingDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
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

        private async Task<User> GetUserByIdAsync(int userId)
        {

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            _guard.NotFoundUser(user);
            return user!;
        }
    }
}
