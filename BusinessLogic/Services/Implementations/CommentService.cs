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
    public class CommentService : ICommentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CommentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Comment>> GetFullCommentsAsync()
        {
            return await _context.Comments
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Comment> GetFullCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == id);

            GuardAgainstInvalidComment(comment);
            return comment;
        }
        public async Task<CommentDTO> GetCommentByIdAsync(int commentId)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == commentId);

            GuardAgainstInvalidComment(comment);
            return _mapper.Map<CommentDTO>(comment);
        }
        public async Task CreateCommentAsync(int userId, int postId, string text)
        {
            var user = await GetUserByIdAsync(userId);
            var post = await GetPostByIdAsync(postId);

            await _context.Comments.AddAsync(
                new Comment
                {
                    UserId = userId,
                    PostId = postId,
                    Text = text
                });

            await _context.SaveChangesAsync();
        }
        public async Task CreateCommentAnswerAsync(int userId, string text, int parentId, int postId)
        {
            var user = await GetUserByIdAsync(userId);
            var post = await GetPostByIdAsync(postId);

            var parent = await _context.Comments
                .SingleOrDefaultAsync(c => c.Id == parentId);

            GuardAgainstInvalidComment(parent);

            await _context.Comments.AddAsync(new Comment
            {
                User = user,
                Text = text,
                Parent = parent,
                Post = post
            });

            await _context.SaveChangesAsync();
        }
        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await GetFullCommentByIdAsync(commentId);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync()
        {
            return await _context.Comments
                .AsNoTracking()
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<CommentDTO>> GetCommentsByUserAsync(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.SubComments)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        private async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            GuardAgainstInvalidUser(user);
            return user;
        }
        private async Task<Post> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .SingleOrDefaultAsync(u => u.Id == postId);

            GuardAgainstInvalidPost(post);
            return post;
        }

        private void GuardAgainstInvalidPost(Post post)
        {
            if (post == null)
            {
                throw new BadRequestException(Common.Resources.PostExceptionMessageResource.PostNotFound);
            }
        }
        private void GuardAgainstInvalidUser(User user)
        {
            if (user == null)
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.UserNotFound);
            }
        }
        private void GuardAgainstInvalidComment(Comment comment)
        {
            if (comment == null)
            {
                throw new NotFoundException(Common.Resources.CommentExceptionMessageResource.CommentNotFound);
            }
        }
    }
}
