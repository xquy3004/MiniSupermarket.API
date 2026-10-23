using MiniSupermarketWinForms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public partial class FormLogin : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private const int EM_SETCUEBANNER = 0x1501;

        private Color _userBorder = Color.FromArgb(209, 213, 219);
        private Color _passBorder = Color.FromArgb(209, 213, 219);

        // Khởi tạo HttpClient trỏ đến địa chỉ của Web API Backend
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000/api/")
        };

        public FormLogin()
        {
            InitializeComponent();
            SetupCustomUI();
        }

        private void SetupCustomUI()
        {
            this.AcceptButton = btnLogin;

            pnlUser.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var r = pnlUser.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                using (var path = GetRoundedRect(r, 6))
                using (var pen = new Pen(_userBorder, 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            pnlPass.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var r = pnlPass.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                using (var path = GetRoundedRect(r, 6))
                using (var pen = new Pen(_passBorder, 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            txtUser.Enter += (s, e) => { _userBorder = Color.FromArgb(37, 99, 235); pnlUser.Invalidate(); };
            txtUser.Leave += (s, e) => { _userBorder = Color.FromArgb(209, 213, 219); pnlUser.Invalidate(); };
            pnlUser.Click += (s, e) => txtUser.Focus();

            txtPass.Enter += (s, e) => { _passBorder = Color.FromArgb(37, 99, 235); pnlPass.Invalidate(); };
            txtPass.Leave += (s, e) => { _passBorder = Color.FromArgb(209, 213, 219); pnlPass.Invalidate(); };
            pnlPass.Click += (s, e) => txtPass.Focus();

            btnLogin.Click += btnLogin_Click;
            btnLogin.Resize += (s, e) => ApplyButtonRoundCorners();

            this.Resize += (s, e) => CenterCard();
            this.Load += FormLogin_Load;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            CenterCard();
            ApplyButtonRoundCorners();
            SendMessage(txtUser.Handle, EM_SETCUEBANNER, 1, "Tài khoản");
            SendMessage(txtPass.Handle, EM_SETCUEBANNER, 1, "Mật khẩu");
        }

        private void CenterCard()
        {
            pnlCard.Location = new Point(
                (this.ClientSize.Width - pnlCard.Width) / 2,
                (this.ClientSize.Height - pnlCard.Height) / 2
            );
        }

        private void ApplyButtonRoundCorners()
        {
            if (btnLogin.Width > 0 && btnLogin.Height > 0)
            {
                using (var path = GetRoundedRect(btnLogin.ClientRectangle, 6))
                {
                    btnLogin.Region = new Region(path);
                }
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Sự kiện khi người dùng bấm nút Đăng nhập
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            // Kiểm tra ràng buộc cơ bản phía Client
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Đóng gói dữ liệu gửi lên endpoint POST /api/auth/login
                var loginData = new { Username = username, Password = password };
                var response = await _client.PostAsJsonAsync("auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    // Đọc chuỗi JSON trả về từ Server khi đăng nhập thành công
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);

                    // Trích xuất Token và Role lưu vào lớp tĩnh SessionManager dùng chung toàn ứng dụng
                    SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                    SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;

                    MessageBox.Show($"Đăng nhập thành công với quyền: {SessionManager.CurrentRole}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở Form quản lý chính (FormCategoryManagement) và ẩn Form đăng nhập đi
                    FormCategoryManagement mainForm = new FormCategoryManagement();
                    this.Hide();
                    mainForm.ShowDialog();
                    this.Close(); // Đóng hẳn ứng dụng khi form chính tắt
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến Server: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
