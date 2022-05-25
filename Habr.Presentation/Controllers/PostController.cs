using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess;
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
                    var response = await _postService.GetPublishedPostsAsync();
                    return response is null || response.Count() == 0 ? NotFound("NotFound") : Ok(response);
                }
                else
                {
                    var response = await _postService.GetNotPublishedPostsAsync();
                    return response is null || response.Count() == 0 ? NotFound("NotFound") : Ok(response);
                }
            }
            else
            {
                var response = await _postService.GetPostsAsync();
                return response is null || response.Count() == 0 ? NotFound("NotFound") : Ok(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(int id)
        {
            try
            {
                return Ok(await _postService.GetPublishedPostByIdAsync(id));
            }
            catch(Exception ex)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetCommentByPost(int id)
        {
            try
            {
                return Ok(await _commentService.GetCommentsByPostAsync(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] int userId, [FromQuery] bool isPublished)
        {
            try
            {
                await _postService.CreatePostAsync(title, text, userId, isPublished);
                return StatusCode(201);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            try
            {
                await _postService.UpdatePostAsync(post);
                return StatusCode(204);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                await _postService.DeletePostAsync(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
