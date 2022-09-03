using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.Common.Parameters;
using Habr.Presentation.Extensions;
using Habr.Presentation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Habr.Presentation.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/users/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentsService;
        private readonly IJwtService _jwtService;

        public UserController(
            IUserService userService, 
            IPostService postService, 
            ICommentService commentService, 
            IJwtService jwtService)
        {
            _userService = userService;
            _postService = postService;
            _commentsService = commentService;
            _jwtService = jwtService;
        }

        [HttpGet, ApiVersionNeutral]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet("{userId}"), ApiVersionNeutral]
        public async Task<IActionResult> GetUserById(int userId)
        {
            return Ok(await _userService.GetUserByIdAsync(userId));
        }

        [HttpGet("{id}/posts"), ApiVersionNeutral]
        public async Task<IActionResult> GetPostByUserId(int id, PostParameters postParameters)
        {
            var posts = await _postService.GetPostsByUserAsync(id, postParameters);

            var metadata = new
            {
                posts.TotalCount,
                posts.PageSize,
                posts.CurrentPage,
                posts.TotalPages,
                posts.HasNext,
                posts.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(posts);
        }

        [HttpGet("{id}/comments"), ApiVersionNeutral]
        public async Task<IActionResult> GetCommentsByUserId(int id)
        {
            return Ok(await _commentsService.GetCommentsByUserAsync(id));
        }

        [HttpPost("sign-up"), ApiVersionNeutral]
        public async Task<IActionResult> SignUp([FromQuery] string name, [FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userService.SignUpAsync(name, email, password);
            var token = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.UpdateRefreshTokenUserAsync(user.Id);
            _jwtService.SetRefreshTokenInCookie(refreshToken.Token, Response);

            var response = new AuthResponseDTO
            {
                AccessToken = token,
                UserData = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate
                }
            };

            return Ok(response);
        }

        [Authorize]
        [HttpDelete, ApiVersionNeutral]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = HttpContext.User.Identity.GetAuthorizedUserId();
            await _userService.DeleteAsync(userId);
            return StatusCode(204);
        }

        [Authorize]
        [HttpPut("edit"), ApiVersionNeutral]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO user)
        {
            var userId = HttpContext.User.Identity.GetAuthorizedUserId();
            await _userService.UpdateAsync(userId, user);
            return StatusCode(204);
        }

        [Authorize]
        [HttpPut("edit/avatar"), ApiVersionNeutral]
        public async Task<IActionResult> UpdateUserAvatar([FromForm] IFormFile image)
        {
            var userId = HttpContext.User.Identity.GetAuthorizedUserId();
            await _userService.UpdateAvatarAsync(userId, image);
            return StatusCode(204);
        }

        [HttpGet("sign-in"), ApiVersionNeutral]
        public async Task<IActionResult> SignIn([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userService.SignInAsync(email, password);
            var token = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _jwtService.UpdateRefreshTokenUserAsync(user.Id);
            _jwtService.SetRefreshTokenInCookie(refreshToken.Token, Response);

            var response = new AuthResponseDTO
            {
                AccessToken = token,
                UserData = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate
                }
            };

            return Ok(response);
        }
    }
}
