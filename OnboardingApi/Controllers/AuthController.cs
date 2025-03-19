using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            if (string.IsNullOrEmpty(response.AccessToken))
                return Ok(new { Message = "Signup successful. Please check your email to confirm your account.", UserId = response.User.Id });

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
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var jwtRole = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            string role = "user"; // Default
            if (jwtRole == "authenticated" && userId == "16bf8214-043e-4a87-b8ae-311e766bc4b5")
            {
                role = "admin"; // Hardcode for you
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            Console.WriteLine("Claims in Verify: " + System.Text.Json.JsonSerializer.Serialize(claims));

            return Ok(new { UserId = userId, Role = role });
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

        // Model for auth.users_metadata
        public class UserMetadata
        {
            public Guid UserId { get; set; }
            public Dictionary<string, string> Metadata { get; set; }
        }

    }
}









//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Supabase;

//namespace OnboardingApi.Controllers
//{
//    [ApiController]
//    [Route("api/auth")]
//    public class AuthController : ControllerBase
//    {
//        private readonly Supabase.Client _supabase;

//        public AuthController(Supabase.Client supabase)
//        {
//            _supabase = supabase;
//        }

//        [HttpPost("signup")]
//        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
//        {
//            var response = await _supabase.Auth.SignUp(request.Email, request.Password);
//            if (response == null || response.User == null)
//                return BadRequest("Signup failed.");

//            if (string.IsNullOrEmpty(response.AccessToken))
//                return Ok(new { Message = "Signup successful. Please check your email to confirm your account.", UserId = response.User.Id });

//            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            var response = await _supabase.Auth.SignIn(request.Email, request.Password);
//            if (response == null || response.User == null)
//                return Unauthorized("Invalid credentials.");

//            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
//        }

//        [HttpGet("verify")]
//        [Authorize]
//        public IActionResult Verify()
//        {
//            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
//            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? "User";

//            var claims = User.Claims.Select(c => new { c.Type, c.Value });
//            Console.WriteLine("Claims in Verify: " + System.Text.Json.JsonSerializer.Serialize(claims));

//            return Ok(new { UserId = userId, Role = role });
//        }
//    }

//    public class SignupRequest
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//    }

//    public class LoginRequest
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//    }
//}