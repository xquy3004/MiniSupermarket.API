using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

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
            this.lblKeyword = new Label();
            this.txtKeyword = new TextBox();
            this.btnSearch = new Button();
            this.dgvCustomers = new DataGridView();

            this.lblCustomerId = new Label();
            this.txtCustomerId = new TextBox();
            this.lblCustomerName = new Label();
            this.txtCustomerName = new TextBox();
            this.lblPhoneNumber = new Label();
            this.txtPhoneNumber = new TextBox();
            this.lblAddress = new Label();
            this.txtAddress = new TextBox();
            this.lblRewardPoints = new Label();
            this.txtRewardPoints = new TextBox();
            this.lblMembershipRank = new Label();
            this.txtMembershipRank = new TextBox();

            this.btnLoad = new Button();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();

            ((ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();

            // Tìm kiếm
            this.lblKeyword.AutoSize = true;
            this.lblKeyword.Location = new Point(20, 20);
            this.lblKeyword.Text = "Tìm kiếm:";

            this.txtKeyword.Location = new Point(90, 17);
            this.txtKeyword.Size = new Size(200, 23);

            this.btnSearch.Location = new Point(300, 15);
            this.btnSearch.Size = new Size(90, 27);
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

            // DataGridView
            this.dgvCustomers.Location = new Point(20, 55);
            this.dgvCustomers.Size = new Size(560, 380);
            this.dgvCustomers.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.CellClick += new DataGridViewCellEventHandler(this.dgvCustomers_CellClick);

            // Cột nhập liệu bên phải
            int labelX = 610;
            int inputX = 700;
            int startY = 55;
            int stepY = 40;

            this.lblCustomerId.AutoSize = true;
            this.lblCustomerId.Location = new Point(labelX, startY);
            this.lblCustomerId.Text = "Mã KH:";
            this.txtCustomerId.Location = new Point(inputX, startY - 3);
            this.txtCustomerId.Size = new Size(150, 23);
            this.txtCustomerId.ReadOnly = true;

            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new Point(labelX, startY + stepY);
            this.lblCustomerName.Text = "Tên KH:";
            this.txtCustomerName.Location = new Point(inputX, startY + stepY - 3);
            this.txtCustomerName.Size = new Size(150, 23);

            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new Point(labelX, startY + stepY * 2);
            this.lblPhoneNumber.Text = "SĐT:";
            this.txtPhoneNumber.Location = new Point(inputX, startY + stepY * 2 - 3);
            this.txtPhoneNumber.Size = new Size(150, 23);

            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new Point(labelX, startY + stepY * 3);
            this.lblAddress.Text = "Địa chỉ:";
            this.txtAddress.Location = new Point(inputX, startY + stepY * 3 - 3);
            this.txtAddress.Size = new Size(150, 23);

            this.lblRewardPoints.AutoSize = true;
            this.lblRewardPoints.Location = new Point(labelX, startY + stepY * 4);
            this.lblRewardPoints.Text = "Điểm t.lũy:";
            this.txtRewardPoints.Location = new Point(inputX, startY + stepY * 4 - 3);
            this.txtRewardPoints.Size = new Size(150, 23);

            this.lblMembershipRank.AutoSize = true;
            this.lblMembershipRank.Location = new Point(labelX, startY + stepY * 5);
            this.lblMembershipRank.Text = "Hạng thẻ:";
            this.txtMembershipRank.Location = new Point(inputX, startY + stepY * 5 - 3);
            this.txtMembershipRank.Size = new Size(150, 23);

            // Nút chức năng
            int btnY = startY + stepY * 6 + 10;

            this.btnLoad.Location = new Point(labelX, btnY);
            this.btnLoad.Size = new Size(240, 32);
            this.btnLoad.Text = "Tải danh sách";
            this.btnLoad.Click += new EventHandler(this.btnLoad_Click);

            this.btnAdd.Location = new Point(labelX, btnY + 40);
            this.btnAdd.Size = new Size(240, 32);
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

            this.btnUpdate.Location = new Point(labelX, btnY + 80);
            this.btnUpdate.Size = new Size(240, 32);
            this.btnUpdate.Text = "Cập nhật";
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);

            this.btnDelete.Location = new Point(labelX, btnY + 120);
            this.btnDelete.Size = new Size(240, 32);
            this.btnDelete.Text = "Xóa";
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // Form
            this.ClientSize = new Size(970, 460);
            this.Text = "Quản lý Khách hàng";

            this.Controls.Add(this.lblKeyword);
            this.Controls.Add(this.txtKeyword);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.lblCustomerId);
            this.Controls.Add(this.txtCustomerId);
            this.Controls.Add(this.lblCustomerName);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lblPhoneNumber);
            this.Controls.Add(this.txtPhoneNumber);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblRewardPoints);
            this.Controls.Add(this.txtRewardPoints);
            this.Controls.Add(this.lblMembershipRank);
            this.Controls.Add(this.txtMembershipRank);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);

            ((ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);
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

        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var customers =
                    await client.GetFromJsonAsync<List<CustomerDto>>("customers");

                dgvCustomers.DataSource = customers;
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

                dgvCustomers.DataSource = result;
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