using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
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
            return Ok(await _commentService.GetCommentsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentsById(int id)
        {
            return Ok(await _commentService.GetCommentByIdAsync(id));
        }

        [Authorize]
        [HttpPost]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.GetFullCommentByIdAsync(id);
            if (comment is not null && (comment.UserId == HttpContext.User.Identity.GetAuthorizedUserId()
                                    || HttpContext.User.Identity.GetAuthorizedUserRole() == Roles.Admin.ToString()))
            {
                await _commentService.DeleteCommentAsync(id);
                return StatusCode(204);
            }
            else
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.AccessError);
            }
        }
    }
}
