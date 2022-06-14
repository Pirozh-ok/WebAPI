﻿using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.Exceptions;
using Habr.DataAccess.Entities;
using Habr.Presentation.Extensions;
using Habr.Presentation.Services;
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

        public object UserExceptionMessageResource { get; private set; }

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
            await _userService.RegisterAsync(name, email, password);
            var user = await _userService.LogInAsync(email, password);

            var token = _jwtService.GenerateAccessToken(user);
            var response = new
            {
                access_token = token,
                userdata = user
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
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user is not null && user.Id == HttpContext.User.Identity.GetAuthorizedUserId())
            {
                await _userService.UpdateAsync(user);
                return StatusCode(204);
            }
            else
            {
                throw new BadRequestException(Common.Resources.UserExceptionMessageResource.AccessError);
            }
        }

        [HttpGet("sign-in")]
        public async Task<IActionResult> SignIn([FromQuery] string email, [FromQuery] string password)
        {
            var user = await _userService.LogInAsync(email, password);

            var token = _jwtService.GenerateAccessToken(user);
            var response = new
            {
                access_token = token,
                userdata = user
            };

            return Ok(response);
        }
    }
}
