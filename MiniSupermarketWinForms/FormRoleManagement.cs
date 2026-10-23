using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    // Màn hình Quản lý Vai trò (Bài mở rộng Buổi 1)
    public class FormRoleManagement : Form
    {
        private DataGridView dgvRoles;
        private TextBox txtId;
        private TextBox txtRoleName;
        private TextBox txtDescription;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormRoleManagement()
        {
            InitializeComponent();
            this.Load += FormRoleManagement_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Vai trò";
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 10F);

            dgvRoles = new DataGridView
            {
                Name = "dgvRoles",
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
            dgvRoles.CellClick += dgvRoles_CellClick;

            var panelDetail = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                AutoScroll = true
            };

            txtId = new TextBox { Name = "txtId", ReadOnly = true, BackColor = Color.FromArgb(235, 235, 235) };
            txtRoleName = new TextBox { Name = "txtRoleName" };
            txtDescription = new TextBox { Name = "txtDescription", Multiline = true, Height = 80 };

            AddField(panelDetail, "Mã vai trò (ID)", txtId, 15);
            AddField(panelDetail, "Tên vai trò", txtRoleName, 75);
            AddField(panelDetail, "Mô tả vai trò", txtDescription, 135);

            btnAdd = CreateButton("Thêm vai trò", Color.FromArgb(39, 174, 96), 245);
            btnUpdate = CreateButton("Cập nhật vai trò", Color.FromArgb(41, 128, 185), 295);
            btnDelete = CreateButton("Xóa vai trò", Color.FromArgb(192, 57, 43), 345);
            btnLoad = CreateButton("Tải lại danh sách", Color.FromArgb(120, 120, 120), 395);

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnLoad.Click += btnLoad_Click;

            panelDetail.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnLoad });

            this.Controls.Add(dgvRoles);
            this.Controls.Add(panelDetail);
            dgvRoles.BringToFront();
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
                Size = new Size(305, 40),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = new HttpClient
            {
                BaseAddress = ApiClientService.Client.BaseAddress ?? new Uri("http://localhost:5000/api/")
            };

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        private async void FormRoleManagement_Load(object sender, EventArgs e)
        {
            await LoadRolesAsync();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var roles = await client.GetFromJsonAsync<List<RoleDto>>("roles");
                dgvRoles.DataSource = roles;

                if (dgvRoles.Columns["Id"] != null) dgvRoles.Columns["Id"].HeaderText = "Mã";
                if (dgvRoles.Columns["RoleName"] != null) dgvRoles.Columns["RoleName"].HeaderText = "Tên vai trò";
                if (dgvRoles.Columns["Description"] != null) dgvRoles.Columns["Description"].HeaderText = "Mô tả";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRoles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvRoles.Rows.Count)
            {
                DataGridViewRow row = dgvRoles.Rows[e.RowIndex];
                txtId.Text = row.Cells["Id"]?.Value?.ToString() ?? string.Empty;
                txtRoleName.Text = row.Cells["RoleName"]?.Value?.ToString() ?? string.Empty;
                txtDescription.Text = row.Cells["Description"]?.Value?.ToString() ?? string.Empty;
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Tên vai trò không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newRole = new
            {
                RoleName = txtRoleName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("roles", newRole);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm vai trò mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRolesAsync();
                    ClearInputs();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm mới thất bại! ({response.StatusCode}): {err}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Vui lòng chọn vai trò cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Tên vai trò không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = int.Parse(txtId.Text);
            var updateRole = new
            {
                Id = id,
                RoleName = txtRoleName.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"roles/{id}", updateRole);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRolesAsync();
                    ClearInputs();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại! ({response.StatusCode}): {err}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Vui lòng chọn vai trò cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = int.Parse(txtId.Text);
            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vai trò [{txtRoleName.Text}]?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"roles/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa vai trò thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRolesAsync();
                    ClearInputs();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa thất bại! ({response.StatusCode}): {err}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtId.Text = string.Empty;
            txtRoleName.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; }
    }
}

