﻿using Habr.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [Route("api/posts/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            try
            {
                return Ok(_postService.GetPublishedPostDTOAsync(id));
            }
            catch(Exception ex)
            {
                return NotFound("Not found");
            }
        }

    }
}
