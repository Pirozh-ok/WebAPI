using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [Route("api/comments/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            var response = await _commentService.GetCommentsAsync();
            return response is null || response.Count() == 0 ? NotFound("NotFound!") : Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentsById(int id)
        {
            try
            {
                return Ok(await _commentService.GetCommentByIdAsync(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromQuery] int userId, [FromQuery] string text, [FromQuery] int postId, [FromQuery] int? parentId)
        {
            try
            {
                if (parentId is null)
                {
                    await _commentService.CreateCommentAsync(userId, postId, text);
                }
                else
                {
                    await _commentService.CreateCommentAnswerAsync(userId, text, (int)parentId, postId);
                }
                return StatusCode(202);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                await _commentService.DeleteCommentAsync(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    } 
}
