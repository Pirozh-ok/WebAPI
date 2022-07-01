using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.Common.Exceptions;
using Habr.Common.Parameters;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/posts/")]
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

        [HttpGet, MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPostsAsyncV1([FromQuery] PostParameters postParameters)
        {
            var posts = await _postService.GetPublishedPostsAsync(postParameters);

            var response = new PagedListDTO<PublishedPostDTO>()
            {
                items = posts,
                TotalCount = posts.TotalCount,
                PageSize = posts.PageSize,
                CurrentPage = posts.CurrentPage,
                TotalPages = posts.TotalPages,
                HasNext = posts.HasNext,
                HasPrevious = posts.HasPrevious
            };

            return Ok(response);
        }

        [HttpGet, MapToApiVersion("2.0")]
        public async Task<IActionResult> GetPostsAsyncV2([FromQuery] PostParameters postParameters)
        {
            var posts = await _postService.GetPublishedPostsAsyncV2(postParameters);

            var response = new PagedListDTO<PublishedPostDTOv2>()
            {
                items = posts,
                TotalCount = posts.TotalCount,
                PageSize = posts.PageSize,
                CurrentPage = posts.CurrentPage,
                TotalPages = posts.TotalPages,
                HasNext = posts.HasNext,
                HasPrevious = posts.HasPrevious
            };

            return Ok(response);
        }

        [HttpGet("my-drafts"), ApiVersionNeutral]
        [Authorize]
        public async Task<IActionResult> GetNotPublishedPostsAsync([FromQuery] PostParameters postParameters)
        {
            var posts = await _postService.GetNotPublishedPostsByUserAsync(
                HttpContext.User.Identity.GetAuthorizedUserId(), 
                postParameters);

            var response = new PagedListDTO<NotPublishedPostDTO>()
            {
                items = posts,
                TotalCount = posts.TotalCount,
                PageSize = posts.PageSize,
                CurrentPage = posts.CurrentPage,
                TotalPages = posts.TotalPages,
                HasNext = posts.HasNext,
                HasPrevious = posts.HasPrevious
            };

            return Ok(response);
        }

        [HttpGet("my-posts"), ApiVersionNeutral]
        [Authorize]
        public async Task<IActionResult> GetPublishedPostsAsync([FromQuery] PostParameters postParameters)
        {
            var posts = await _postService.GetPublishedPostsByUserAsync(
                HttpContext.User.Identity.GetAuthorizedUserId(),
                postParameters);

            var response = new PagedListDTO<PublishedPostDTO>()
            {
                items = posts,
                TotalCount = posts.TotalCount,
                PageSize = posts.PageSize,
                CurrentPage = posts.CurrentPage,
                TotalPages = posts.TotalPages,
                HasNext = posts.HasNext,
                HasPrevious = posts.HasPrevious
            };

            return Ok(response);
        }

        [HttpGet("{id}"), MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPostByIdAsyncV1(int id)
        {
            return Ok(await _postService.GetPublishedPostByIdAsync(id));
        }

        [HttpGet("{id}"), MapToApiVersion("2.0")]
        public async Task<IActionResult> GetPostByIdAsyncV2(int id)
        {
            return Ok(await _postService.GetPublishedPostByIdAsyncV2(id));
        }

        [HttpGet("{id}/comments"), ApiVersionNeutral]
        public async Task<IActionResult> GetCommentByPost(int id)
        {
            return Ok(await _commentService.GetCommentsByPostAsync(id));
        }

        [Authorize]
        [HttpPost, ApiVersionNeutral]
        public async Task<IActionResult> CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] bool isPublished)
        {

            await _postService.CreatePostAsync(title, text, HttpContext.User.Identity.GetAuthorizedUserId(), isPublished);
            return StatusCode(201);
        }

        [Authorize]
        [HttpPut, ApiVersionNeutral]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            if (post is not null && (post.UserId == HttpContext.User.Identity.GetAuthorizedUserId() 
                                 || HttpContext.User.Identity.GetAuthorizedUserRole() == Roles.Admin.ToString()))
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
        [HttpDelete("{id}"), ApiVersionNeutral]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postService.GetFullPostByIdAsync(id);
            if (post is not null && (post.UserId == HttpContext.User.Identity.GetAuthorizedUserId()
                                 || HttpContext.User.Identity.GetAuthorizedUserRole() == Roles.Admin.ToString()))
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
