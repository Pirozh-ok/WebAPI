﻿using Habr.BusinessLogic.Services.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.Common.Parameters;
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
        public async Task<IActionResult> CreatePost([FromQuery] string title, [FromQuery] string text, [FromQuery] bool isPublished, List<IFormFile> images)
        {

            await _postService.CreatePostAsync(title, text, HttpContext.User.Identity.GetAuthorizedUserId(), isPublished, images);
            return StatusCode(201);
        }

        [Authorize]
        [HttpPut, ApiVersionNeutral]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostDTO post)
        {
            await _postService.UpdatePostAsync(HttpContext.User.Identity.GetAuthorizedUserId(), post);
            return StatusCode(204);
        }

        [Authorize]
        [HttpDelete("{postId}"), ApiVersionNeutral]
        public async Task<IActionResult> DeletePost(int postId)
        {

            await _postService.DeletePostAsync(postId, HttpContext.User.Identity.GetAuthorizedUserId());
            return StatusCode(204);
        }

        [Authorize]
        [HttpPost("rating"), ApiVersionNeutral]
        public async Task<IActionResult> AddPostRating([FromQuery] int postId, [FromQuery] int rate)
        {
            await _postService.RatePost(postId, HttpContext.User.Identity.GetAuthorizedUserId(), rate);
            return Ok();
        }

        [HttpGet("{postId}/ratings"), ApiVersionNeutral]
        public async Task<IActionResult> GetPostRatingsByPostId(int postId)
        {      
            return Ok(await _postService.GetRatingsByPostId(postId));
        }

        [HttpPut("{postId}/send-to-drafts"), ApiVersionNeutral]
        public async Task<IActionResult> SendPostToDrafts(int postId)
        {
            await _postService.SendPostToDraftsAsync(HttpContext.User.Identity.GetAuthorizedUserId(), postId); 
            return Ok();
        }

        [HttpPut("{postId}/publish"), ApiVersionNeutral]
        public async Task<IActionResult> PublishPost(int postId)
        {
            await _postService.PublishPostAsync(postId, HttpContext.User.Identity.GetAuthorizedUserId());
            return Ok();
        }
    }
}
