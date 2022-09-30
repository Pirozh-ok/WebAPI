using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/comments/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet, ApiVersionNeutral]
        public async Task<IActionResult> GetComments()
        {
            return Ok(await _commentService.GetCommentsAsync());
        }

        [HttpGet("{id}"), ApiVersionNeutral]
        public async Task<IActionResult> GetCommentsById(int id)
        {
            return Ok(await _commentService.GetCommentByIdAsync(id));
        }

        [Authorize]
        [HttpPost, ApiVersionNeutral]
        public async Task<IActionResult> CreateComment([FromQuery] string text, [FromQuery] int postId, [FromQuery] int? parentId)
        {
            int userId = HttpContext.User.Identity.GetAuthorizedUserId();

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

        [Authorize]
        [HttpDelete("{commentId}"), ApiVersionNeutral]
        public async Task<IActionResult> DeleteComment(int commentId)
        {

            await _commentService.DeleteCommentAsync(commentId, HttpContext.User.Identity.GetAuthorizedUserId());
            return StatusCode(204);
        }
    }
}
