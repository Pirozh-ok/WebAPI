using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [Route("api/posts/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostController(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] bool? isPublished)
        {
            if (isPublished is not null)
            {
                if ((bool)isPublished)
                {
                    return Ok(await _postService.GetPublishedPostsAsync());
                }
                else
                {
                    return Ok(await _postService.GetNotPublishedPostsAsync());
                }
            }
            else
            {
                return Ok(await _postService.GetPostsAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(int id)
        {

            return Ok(await _postService.GetPublishedPostByIdAsync(id));
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetCommentByPost(int id)
        {
            return Ok(await _commentService.GetCommentsByPostAsync(id));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] bool isPublished)
        {

            await _postService.CreatePostAsync(title, text, HttpContext.User.Identity.GetAuthorizedUserId(), isPublished);
            return StatusCode(201);

        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            if (post is not null && post.UserId == HttpContext.User.Identity.GetAuthorizedUserId())
            {
                await _postService.UpdatePostAsync(post);
                return StatusCode(204);
            }
            else
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.AccessError);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postService.GetFullPostByIdAsync(id);
            if (post is not null && post.UserId == HttpContext.User.Identity.GetAuthorizedUserId())
            {
                await _postService.DeletePostAsync(id);
                return StatusCode(204);
            }
            else
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.AccessError);
            }
        }
    }
}
