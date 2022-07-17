using Habr.Presentation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/tokens/")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public TokenController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("refresh-token"), ApiVersionNeutral]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if(refreshToken is null)
            {
                return NotFound();
            }

            var response = await _jwtService.RefreshJwtToken(refreshToken, Response);
            return Ok(response);
        }
    }
}
