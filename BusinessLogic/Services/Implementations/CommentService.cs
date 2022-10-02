using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Guards.Interfaces;
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
        private readonly ICommentGuard _guard; 

        public CommentService(DataContext context, IMapper mapper, ICommentGuard guard)
        {
            _context = context;
            _mapper = mapper;
            _guard = guard;
        }

        public async Task<IEnumerable<Comment>> GetFullCommentsAsync()
        {
            var comments =  await _context.Comments
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListComment(comments);
            return comments;
        }

        public async Task<Comment> GetFullCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == id);

            _guard.NotFoundComment(comment);
            return comment!;
        }

        public async Task<CommentDTO> GetCommentByIdAsync(int commentId)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == commentId);

            _guard.NotFoundComment(comment);
            return _mapper.Map<CommentDTO>(comment);
        }

        public async Task CreateCommentAsync(int userId, int postId, string text)
        {
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

            _guard.NotFoundComment(parent);

            await _context.Comments.AddAsync(new Comment
            {
                User = user,
                Text = text,
                Parent = parent,
                Post = post
            });

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await GetFullCommentByIdAsync(commentId);
            _guard.NotFoundComment(comment);
            _guard.AccessError(userId, comment); 
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync()
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            _guard.InvalidListComment(comments);
            return comments;
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.SubComments)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListComment(comments);
            return comments;
        }
        public async Task<IEnumerable<CommentDTO>> GetCommentsByUserAsync(int userId)
        {
            var comments = await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.SubComments)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            _guard.InvalidListComment(comments);
            return comments;
        }

        private async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            _guard.NotFoundUser(user);
            return user!;
        }

        private async Task<Post> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts
                .SingleOrDefaultAsync(u => u.Id == postId);

            _guard.NotFoundPost(post); 
            return post!;
        }
    }
}
