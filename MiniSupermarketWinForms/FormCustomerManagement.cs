using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MiniSupermarketWinForms
{
    public class FormCustomerManagement : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // ===== Controls =====
        private Label lblKeyword;
        private TextBox txtKeyword;
        private Button btnSearch;
        private DataGridView dgvCustomers;

        private Label lblCustomerId;
        private TextBox txtCustomerId;
        private Label lblCustomerName;
        private TextBox txtCustomerName;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblRewardPoints;
        private TextBox txtRewardPoints;
        private Label lblMembershipRank;
        private TextBox txtMembershipRank;

        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormCustomerManagement()
        {
            InitializeComponent();
            this.Load += FormCustomerManagement_Load;
        }

        // ===== Thiết kế giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Quản lý Khách hàng";
            this.Size = new Size(1100, 600);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 10F);

            // --- Thanh tìm kiếm (trên cùng) ---
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };

            lblKeyword = new Label { Text = "Tìm kiếm:", AutoSize = true, Location = new Point(15, 20) };
            txtKeyword = new TextBox { Location = new Point(95, 17), Width = 260 };
            txtKeyword.KeyDown += txtKeyword_KeyDown;
            btnSearch = CreateButton("Tìm kiếm", Color.FromArgb(41, 100, 180), 370, 14, 110, 32);
            btnSearch.Click += btnSearch_Click;

            panelTop.Controls.AddRange(new Control[] { lblKeyword, txtKeyword, btnSearch });

            // --- Bảng khách hàng (lấp đầy vùng còn lại) ---
            dgvCustomers = new DataGridView
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
            dgvCustomers.CellClick += dgvCustomers_CellClick;

            // --- Khung nhập liệu (bên phải) ---
            var panelDetail = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                AutoScroll = true
            };

            txtCustomerId = new TextBox { ReadOnly = true };
            txtCustomerName = new TextBox();
            txtPhoneNumber = new TextBox();
            txtAddress = new TextBox();
            txtRewardPoints = new TextBox();
            txtMembershipRank = new TextBox();

            lblCustomerId = AddField(panelDetail, "Mã KH:", txtCustomerId, 20);
            lblCustomerName = AddField(panelDetail, "Tên KH:", txtCustomerName, 65);
            lblPhoneNumber = AddField(panelDetail, "SĐT:", txtPhoneNumber, 110);
            lblAddress = AddField(panelDetail, "Địa chỉ:", txtAddress, 155);
            lblRewardPoints = AddField(panelDetail, "Điểm tích lũy:", txtRewardPoints, 200);
            lblMembershipRank = AddField(panelDetail, "Hạng thẻ:", txtMembershipRank, 245);

            btnLoad = CreateButton("Tải danh sách", Color.FromArgb(120, 120, 120), 15, 310, 305, 36);
            btnAdd = CreateButton("Thêm mới", Color.FromArgb(39, 174, 96), 15, 354, 305, 36);
            btnUpdate = CreateButton("Cập nhật", Color.FromArgb(41, 100, 180), 15, 398, 305, 36);
            btnDelete = CreateButton("Xóa", Color.FromArgb(192, 57, 43), 15, 442, 305, 36);
            btnLoad.Click += btnLoad_Click;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            panelDetail.Controls.AddRange(new Control[] { btnLoad, btnAdd, btnUpdate, btnDelete });

            // --- Ghép vào Form (thứ tự quyết định cách Dock) ---
            this.Controls.Add(dgvCustomers);
            this.Controls.Add(panelDetail);
            this.Controls.Add(panelTop);
            dgvCustomers.BringToFront();
        }

        // Thêm một cặp nhãn + ô nhập vào khung bên phải
        private Label AddField(Panel parent, string caption, TextBox input, int top)
        {
            var lbl = new Label { Text = caption, AutoSize = true, Location = new Point(15, top + 4) };
            input.Location = new Point(125, top);
            input.Width = 195;
            parent.Controls.Add(lbl);
            parent.Controls.Add(input);
            return lbl;
        }

        private Button CreateButton(string text, Color color, int x, int y, int w, int h)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Cursor = Cursors.Hand
            };
            return btn;
        }

        // ===== Xử lý logic =====

        private HttpClient GetAuthenticatedClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseAddress)
            };

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        private async void FormCustomerManagement_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        // Đổ dữ liệu vào bảng rồi đặt tiêu đề cột tiếng Việt
        private void BindGrid(List<CustomerDto> customers)
        {
            dgvCustomers.DataSource = customers;

            SetHeader("CustomerId", "Mã KH");
            SetHeader("CustomerName", "Tên khách hàng");
            SetHeader("PhoneNumber", "Số điện thoại");
            SetHeader("Address", "Địa chỉ");
            SetHeader("RewardPoints", "Điểm tích lũy");
            SetHeader("MembershipRank", "Hạng thẻ");
        }

        private void SetHeader(string columnName, string text)
        {
            var col = dgvCustomers.Columns[columnName];
            if (col != null) col.HeaderText = text;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var customers =
                    await client.GetFromJsonAsync<List<CustomerDto>>("customers");

                BindGrid(customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kết nối đến Server!\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

            txtCustomerId.Text = row.Cells["CustomerId"].Value?.ToString() ?? "";
            txtCustomerName.Text = row.Cells["CustomerName"].Value?.ToString() ?? "";
            txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value?.ToString() ?? "";
            txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
            txtRewardPoints.Text = row.Cells["RewardPoints"].Value?.ToString() ?? "";
            txtMembershipRank.Text = row.Cells["MembershipRank"].Value?.ToString() ?? "";
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên và Số điện thoại!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newCustomer = new
                {
                    CustomerName = txtCustomerName.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    RewardPoints = int.TryParse(txtRewardPoints.Text, out int rp) ? rp : 0,
                    MembershipRank = string.IsNullOrWhiteSpace(txtMembershipRank.Text)
                        ? "Chuẩn" : txtMembershipRank.Text.Trim()
                };

                using var client = GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("customers", newCustomer);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm khách hàng thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Thêm thất bại!\n\n" + message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var updateCustomer = new
                {
                    CustomerId = id,
                    CustomerName = txtCustomerName.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    RewardPoints = int.TryParse(txtRewardPoints.Text, out int rp) ? rp : 0,
                    MembershipRank = txtMembershipRank.Text.Trim()
                };

                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"customers/{id}", updateCustomer);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Cập nhật thất bại!\n\n" + message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa khách hàng ID = {id}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"customers/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Xóa thất bại!\n\n" + message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nhấn Enter trong ô tìm kiếm cũng tìm luôn
        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSearch.PerformClick();
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                string url = $"customers/search?keyword={Uri.EscapeDataString(keyword)}";

                using var client = GetAuthenticatedClient();
                var result = await client.GetFromJsonAsync<List<CustomerDto>>(url);

                BindGrid(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tìm kiếm!\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            txtRewardPoints.Clear();
            txtMembershipRank.Clear();
        }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = string.Empty;
    }
}