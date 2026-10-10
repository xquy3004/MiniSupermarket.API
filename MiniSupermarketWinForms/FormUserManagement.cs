using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormUserManagement : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        private DataGridView dgvUsers;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtPassword;
        private ComboBox cboRole;
        private Button btnAddUser;
        private Button btnResetPassword;
        private Button btnToggleLock;
        private Button btnLoad;

        // Tài khoản đang được chọn trong bảng
        private int _selectedUserId = 0;
        private string _selectedUsername = string.Empty;

        public FormUserManagement()
        {
            InitializeComponent();
            cboRole.Items.AddRange(new object[] { "Admin", "Cashier", "Warehouse" });
            cboRole.SelectedIndex = 1;
            this.Load += FormUserManagement_Load;
        }

        // ===== Dựng giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Quản trị Tài khoản";
            this.Size = new Size(1100, 600);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 10F);

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            dgvUsers.CellClick += dgvUsers_CellClick;
            dgvUsers.DataBindingComplete += dgvUsers_DataBindingComplete;

            var panelDetail = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                AutoScroll = true
            };

            txtUsername = new TextBox();
            txtFullName = new TextBox();
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            cboRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            AddField(panelDetail, "Tên đăng nhập", txtUsername, 10);
            AddField(panelDetail, "Họ và tên", txtFullName, 68);
            AddField(panelDetail, "Mật khẩu", txtPassword, 126);
            AddField(panelDetail, "Vai trò", cboRole, 184);

            btnAddUser = CreateButton("Thêm tài khoản", Color.FromArgb(39, 174, 96), 262);
            btnResetPassword = CreateButton("Đặt lại mật khẩu", Color.FromArgb(230, 126, 34), 312);
            btnToggleLock = CreateButton("Khóa / Mở khóa", Color.FromArgb(192, 57, 43), 362);
            btnLoad = CreateButton("Tải lại danh sách", Color.FromArgb(120, 120, 120), 412);
            btnAddUser.Click += btnAddUser_Click;
            btnResetPassword.Click += btnResetPassword_Click;
            btnToggleLock.Click += btnToggleLock_Click;
            btnLoad.Click += btnLoad_Click;

            var lblHint = new Label
            {
                Text = "Đặt lại mật khẩu: chọn tài khoản trong bảng, nhập mật khẩu mới vào ô Mật khẩu " +
                       "(để trống = 123456). Dòng màu đỏ là tài khoản đang bị khóa.",
                Location = new Point(15, 464),
                Size = new Size(305, 90),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            panelDetail.Controls.AddRange(new Control[] { btnAddUser, btnResetPassword, btnToggleLock, btnLoad, lblHint });

            this.Controls.Add(dgvUsers);
            this.Controls.Add(panelDetail);
            dgvUsers.BringToFront();
        }

        private void AddField(Panel parent, string caption, Control input, int top)
        {
            var lbl = new Label { Text = caption, AutoSize = true, Location = new Point(15, top) };
            input.Location = new Point(15, top + 24);
            input.Width = 305;
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            parent.Controls.Add(lbl);
            parent.Controls.Add(input);
        }

        private Button CreateButton(string text, Color color, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(15, y),
                Size = new Size(305, 40)
            };
            return btn;
        }

        // ===== Gọi API =====
        private HttpClient GetAuthenticatedClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(ApiBaseAddress) };

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        private async void FormUserManagement_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var users = await client.GetFromJsonAsync<List<UserDto>>("users");

                dgvUsers.DataSource = users;
                SetHeader("Id", "Mã");
                SetHeader("Username", "Tên đăng nhập");
                SetHeader("FullName", "Họ và tên");
                SetHeader("Role", "Vai trò");
                SetHeader("IsActive", "Hoạt động");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetHeader(string columnName, string text)
        {
            var col = dgvUsers.Columns[columnName];
            if (col != null) col.HeaderText = text;
        }

        // Tô đỏ nhạt các dòng tài khoản đang bị khóa
        private void dgvUsers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                var u = row.DataBoundItem as UserDto;
                row.DefaultCellStyle.BackColor = (u != null && !u.IsActive)
                    ? Color.FromArgb(255, 220, 220)
                    : Color.White;
            }
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var u = dgvUsers.Rows[e.RowIndex].DataBoundItem as UserDto;
            if (u == null) return;

            _selectedUserId = u.Id;
            _selectedUsername = u.Username;

            txtUsername.Text = u.Username;
            txtFullName.Text = u.FullName;
            txtPassword.Clear();
            cboRole.SelectedItem = u.Role;
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Tên đăng nhập và mật khẩu không được trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newUser = new
                {
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Role = cboRole.SelectedItem?.ToString() ?? "Cashier"
                };

                using var client = GetAuthenticatedClient();
                var res = await client.PostAsJsonAsync("users", newUser);

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Tạo tài khoản mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowFailureAsync(res, "Tạo tài khoản");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản trong bảng trước!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPass = txtPassword.Text.Trim();
            bool useDefault = newPass.Length == 0;
            if (useDefault) newPass = "123456";

            string detail = useDefault ? "mật khẩu mặc định 123456" : "mật khẩu mới bạn vừa nhập";
            var confirm = MessageBox.Show($"Đặt lại mật khẩu của \"{_selectedUsername}\" thành {detail}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.PutAsJsonAsync($"users/{_selectedUserId}/reset-password",
                    new { NewPassword = newPass });

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã đặt lại mật khẩu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPassword.Clear();
                }
                else
                {
                    await ShowFailureAsync(res, "Đặt lại mật khẩu");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnToggleLock_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản trong bảng trước!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Đổi trạng thái khóa / mở khóa của tài khoản \"{_selectedUsername}\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.PutAsync($"users/{_selectedUserId}/toggle-lock", null);

                if (res.IsSuccessStatusCode)
                {
                    string body = await res.Content.ReadAsStringAsync();
                    MessageBox.Show(ExtractMessage(body), "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUsersAsync();
                }
                else
                {
                    await ShowFailureAsync(res, "Khóa / mở khóa");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ShowFailureAsync(HttpResponseMessage res, string action)
        {
            if (res.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show($"{action} thất bại: bạn không có quyền thực hiện thao tác này!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string body = await res.Content.ReadAsStringAsync();
            MessageBox.Show($"{action} thất bại!\n\n{ExtractMessage(body)}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Server trả JSON dạng {"message":"..."}, lấy riêng phần nội dung để hiển thị
        private static string ExtractMessage(string body)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var m))
                {
                    return m.GetString() ?? body;
                }
            }
            catch
            {
            }
            return body;
        }

        private void ClearInputs()
        {
            _selectedUserId = 0;
            _selectedUsername = string.Empty;
            txtUsername.Clear();
            txtFullName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 1;
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}