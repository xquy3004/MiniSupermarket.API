using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarketWinForms
{
    // Lớp tĩnh quản lý trạng thái phiên đăng nhập của ứng dụng (lưu Token)
    public static class SessionManager
    {
        // Lưu trữ JWT Token nhận được từ Backend
        public static string JwtToken { get; set; } = string.Empty;
        // Lưu trữ chức vụ/vai trò hiện tại của người dùng (VD: Admin, Cashier)
        public static string CurrentRole { get; set; } = string.Empty;
    }

    // Lớp tĩnh cung cấp các hàm gọi API dùng chung
    public static class ApiClientService
    {
        // Khởi tạo một thể hiện HttpClient dùng chung xuyên suốt ứng dụng
        private static readonly HttpClient _client = new HttpClient
        {
            // Thiết lập địa chỉ gốc của Backend API (Thay thế port nếu backend của bạn chạy cổng khác)
            BaseAddress = new Uri("http://localhost:5000/api/")
        };

        // Hàm gọi API đăng nhập và lấy Token
        public static async Task<bool> LoginAsync(string username, string password)
        {
            // Đóng gói thông tin đăng nhập vào một object
            var loginObj = new { Username = username, Password = password };

            // Gửi request POST tới endpoint auth/login
            var response = await _client.PostAsJsonAsync("auth/login", loginObj);

            if (response.IsSuccessStatusCode)
            {
                // Nếu đăng nhập thành công, đọc kết quả JSON trả về
                var jsonString = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(jsonString))
                {
                    // Trích xuất Token và vai trò từ JSON và lưu vào SessionManager
                    SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                    SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                }

                return true; // Đăng nhập thành công
            }

            return false; // Đăng nhập thất bại
        }

        // Hàm gọi API lấy dữ liệu tĩnh, có gắn sẵn mã Token để Backend xác thực
        public static async Task<string> GetDataWithTokenAsync(string endpoint)
        {
            // Gắn mã Token vào tiêu đề Authorization của mỗi Request
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);

            // Gọi phương thức GET tới endpoint tương ứng
            var response = await _client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                // Nếu gọi thành công, trả về dữ liệu nguyên bản dạng chuỗi
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Nếu Backend trả về 401 Unauthorized, nghĩa là mã Token đã hết hạn hoặc không hợp lệ
                throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
            }

            throw new Exception("Lỗi khi gọi dữ liệu từ Server.");
        }
    }
}