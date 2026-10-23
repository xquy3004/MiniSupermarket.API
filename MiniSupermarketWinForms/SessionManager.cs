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
        // Lưu tên đăng nhập hiện tại để hiển thị trên thanh tiêu đề
        public static string CurrentUsername { get; set; } = string.Empty;
    }

    // Lớp tĩnh cung cấp các hàm gọi API dùng chung (Buổi 5 + Buổi 6)
    public static class ApiClientService
    {
        // HttpClient dùng chung cho toàn ứng dụng (Buổi 6)
        public static readonly HttpClient Client = new HttpClient
        {
            // Thiết lập địa chỉ gốc của Backend API
            BaseAddress = new Uri("http://localhost:5000/api/")
        };

        // Cập nhật Bearer Token sau khi đăng nhập thành công (Buổi 6)
        public static void AttachToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Alias cho AttachToken (tương thích cũ)
        public static void SetBearerToken(string token) => AttachToken(token);

        // Hàm gọi API đăng nhập và lấy Token (dùng bởi SessionManager gọi trực tiếp)
        public static async Task<bool> LoginAsync(string username, string password)
        {
            var loginObj = new { Username = username, Password = password };
            var response = await Client.PostAsJsonAsync("auth/login", loginObj);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(jsonString))
                {
                    SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                    SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                }

                AttachToken(SessionManager.JwtToken);
                return true;
            }

            return false;
        }

        // Hàm gọi API lấy dữ liệu tĩnh, gắn sẵn Token để Backend xác thực
        public static async Task<string> GetDataWithTokenAsync(string endpoint)
        {
            var response = await Client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
            }

            throw new Exception("Lỗi khi gọi dữ liệu từ Server.");
        }
    }
}