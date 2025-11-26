using CinePass.Core.Services;
using CinePass.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (result == "Success")
            {
                return Ok(new { message = "Đăng ký thành công" });
            }

            return BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Shared.DTOs.Auth.LoginRequest request)
        {
            var tokens = await _authService.LoginAsync(request);

            if (tokens == null)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu" });

            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] Shared.DTOs.Auth.RefreshRequest request)
        {
            var tokens = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (tokens == null)
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn" });

            return Ok(tokens);
        }
    }
}
