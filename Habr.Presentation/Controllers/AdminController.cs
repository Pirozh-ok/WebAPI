using Habr.Presentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.Presentation.Controllers
{

    [Route("api/admins/")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewAdmin([FromQuery] string name, [FromQuery] string email, [FromQuery] string password)
        {
            await _adminService.RegisterAdminAsync(name,email, password);
            return Ok( new
            {
                message = "Administrator has been added!"
            });
        }
    }
}
