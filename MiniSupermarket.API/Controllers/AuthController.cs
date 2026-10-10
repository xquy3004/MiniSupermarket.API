using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SupermarketDbContext _context;

        public AuthController(IConfiguration configuration, SupermarketDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            string username = (request.Username ?? string.Empty).Trim();
            string password = request.Password ?? string.Empty;

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);

            // Sai tên đăng nhập hoặc mật khẩu: trả 401, không tiết lộ cái nào sai
            if (user == null || !PasswordHelper.Verify(password, user.PasswordHash))
            {
                return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
            }

            // Đúng mật khẩu nhưng tài khoản đã bị khóa: trả 403
            if (!user.IsActive)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { success = false, message = "Tài khoản đã bị khóa!" });
            }

            var token = GenerateJwtToken(user.Username, user.Role);
            return Ok(new
            {
                success = true,
                token = token,
                role = user.Role,
                username = user.Username,
                fullName = user.FullName
            });
        }

        // Hàm dùng để tạo JWT Token
        private string GenerateJwtToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // Lớp Dto dùng để hứng dữ liệu đăng nhập từ client
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}