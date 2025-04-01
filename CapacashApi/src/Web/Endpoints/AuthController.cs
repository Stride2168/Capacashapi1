using Capacash.Application.Common.Interfaces;
using Capacash.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Capacash.Web.Endpoints
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserAuthService _userAuthService;
        private readonly IUserRepository _userRepository; // ✅ Injected repository

        public AuthController(UserAuthService userAuthService, IUserRepository userRepository)
        {
            _userAuthService = userAuthService ?? throw new ArgumentNullException(nameof(userAuthService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var token = await _userAuthService.RegisterUserAsync(request.FullName, request.Email, request.Password, request.CompanyId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto dto)
        {
            try
            {
                // 🔍 Check if an admin exists for the given CompanyId
                var adminExists = await _userRepository.ExistsAdminForCompanyAsync(dto.CompanyId);
                if (!adminExists)
                {
                    return BadRequest(new { Error = "No company found with the provided Company ID." });
                }

                var token = await _userAuthService.RegisterUserAsync(dto.FullName, dto.Email, dto.Password, dto.CompanyId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _userAuthService.LoginAsync(request.Email, request.Password);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            try
            {
                var token = await _userAuthService.RegisterAdminAsync(dto.FullName, dto.Email, dto.Password, dto.CompanyId);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class RegisterRequest
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string CompanyId { get; set; } = string.Empty; // Added Company ID
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
