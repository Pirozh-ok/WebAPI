using Habr.BusinessLogic.Services.Interfaces;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [Route("api/users/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentsService;
        public UserController(IUserService userService, IPostService postService, ICommentService commentService)
        {
            _userService = userService;
            _postService = postService;
            _commentsService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await _userService.GetUsersAsync();
            return response is null || response.Count() == 0 ? NotFound("NotFound") : Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                return Ok(await _userService.GetUserById(userId));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPostByUserId(int id)
        {
            try
            {
                return Ok(await _postService.GetPostsByUserAsync(id));
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetCommentsByUserId(int id)
        {
            try
            {
                return Ok(await _commentsService.GetCommentsByUserAsync(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromQuery] string name, [FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                await _userService.RegisterAsync(name, email, password);
                return StatusCode(201);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return StatusCode(204);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                await _userService.UpdateAsync(user);
                return StatusCode(204);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
