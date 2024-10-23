using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avans.FoodWaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication for all actions by default
    public class AuthTestController : ControllerBase
    {
        // This endpoint requires authentication
        [HttpGet("secure")]
        public IActionResult SecureEndpoint()
        {
            return Ok(new { message = "Secure endpoint is accessible." });
        }

        // This endpoint is accessible without authentication
        [HttpGet("public")]
        [AllowAnonymous] // Overrides the class-level [Authorize]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "Public endpoint is accessible." });
        }
    }
}