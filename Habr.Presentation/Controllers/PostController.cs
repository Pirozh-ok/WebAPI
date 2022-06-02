using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
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
        public async Task<IActionResult> CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] int userId, [FromQuery] bool isPublished)
        {

            await _postService.CreatePostAsync(title, text, userId, isPublished);
            return StatusCode(201);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            await _postService.UpdatePostAsync(post);
            return StatusCode(204);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _postService.DeletePostAsync(id);
            return StatusCode(204);
        }
    }
}
