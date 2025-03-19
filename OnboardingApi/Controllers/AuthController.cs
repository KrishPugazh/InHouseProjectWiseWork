using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace OnboardingApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public AuthController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var response = await _supabase.Auth.SignUp(request.Email, request.Password);
            if (response == null || response.User == null)
                return BadRequest("Signup failed.");

            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _supabase.Auth.SignIn(request.Email, request.Password);
            if (response == null || response.User == null)
                return Unauthorized("Invalid credentials.");

            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
        }

        [HttpGet("verify")]
        [Authorize]
        public IActionResult Verify()
        {
            var userId = User.FindFirst("sub")?.Value; // "sub" claim contains user ID
            var role = User.FindFirst("role")?.Value ?? "User"; // Default to "User" if no role
            return Ok(new { UserId = userId, Role = role });
        }
    }

    public class SignupRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
