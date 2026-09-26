using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        // Constructor inject IConfiguration để đọc thiết lập từ appsettings.json
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            // Kiểm tra tài khoản mẫu (Trong thực tế sẽ truy vấn qua EF Core / SQL Server)
            if (request.Username == "admin" && request.Password == "123456")
            {
                // Nếu đúng tài khoản admin, gọi hàm để tạo JWT Token với vai trò "Admin"
                var token = GenerateJwtToken(request.Username, "Admin");
                // Trả về kết quả HTTP 200 OK kèm theo token và vai trò
                return Ok(new { success = true, token = token, role = "Admin" });
            }
            else if (request.Username == "cashier" && request.Password == "123456")
            {
                // Nếu đúng tài khoản cashier, gọi hàm để tạo JWT Token với vai trò "Cashier"
                var token = GenerateJwtToken(request.Username, "Cashier");
                // Trả về kết quả HTTP 200 OK kèm theo token và vai trò
                return Ok(new { success = true, token = token, role = "Cashier" });
            }

            // Nếu sai tài khoản hoặc mật khẩu, trả về lỗi HTTP 401 (Unauthorized)
            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
        }

        // Hàm dùng để tạo JWT Token
        private string GenerateJwtToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Lấy khóa bí mật từ appsettings.json (Nếu không có, sẽ lấy chuỗi mặc định, khóa này nên bảo mật trong thực tế)
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!");

            // Cấu hình các thông tin (claims) và thời hạn của token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username), // Lưu tên đăng nhập vào token
                    new Claim(ClaimTypes.Role, role)      // Lưu vai trò vào token
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Thời hạn token là 2 tiếng kể từ lúc tạo
                // Khai báo thuật toán mã hóa được sử dụng để ký token (HS256)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Tạo ra đối tượng Token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Viết Token thành dạng chuỗi string (mã hóa JWT) để trả về client
            return tokenHandler.WriteToken(token);
        }
    }

    // Lớp Dto (Data Transfer Object) dùng để hứng dữ liệu đầu vào từ phía client
    public class LoginRequestDto
    {
        // Thuộc tính tên người dùng
        public string Username { get; set; } = string.Empty;
        // Thuộc tính mật khẩu
        public string Password { get; set; } = string.Empty;
    }
}