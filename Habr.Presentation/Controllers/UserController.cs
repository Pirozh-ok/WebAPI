using System.Security.Claims;
using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using Habr.Presentation.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IPostService postService, ICommentService commentService, IJwtService jwtService)
        {
            _userService = userService;
            _postService = postService;
            _commentsService = commentService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            return Ok(await _userService.GetUserById(userId));
        }

        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPostByUserId(int id)
        {
            return Ok(await _postService.GetPostsByUserAsync(id));
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetCommentsByUserId(int id)
        {
            return Ok(await _commentsService.GetCommentsByUserAsync(id));
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromQuery] string name, [FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userService.RegisterAsync(name, email, password);
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
        [HttpDelete()]
        public async Task<IActionResult> DeleteUser()
        {
            await _userService.DeleteAsync(HttpContext.User.Identity.GetAuthorizedUserId());
            return StatusCode(204);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO user)
        {
            await _userService.UpdateAsync(HttpContext.User.Identity.GetAuthorizedUserId(), user);
            return StatusCode(204);
        }

        [HttpGet("sign-in")]
        public async Task<IActionResult> SignIn([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userService.LogInAsync(email, password);
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
