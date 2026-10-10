using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormProductManagement : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // Thanh tìm kiếm / lọc
        private TextBox txtSearchBarcode;
        private ComboBox cboFilterCategory;
        private Button btnSearch;

        private DataGridView dgvProducts;

        // Khu vực chi tiết bên phải
        private TextBox txtId;
        private TextBox txtBarcode;
        private TextBox txtProductName;
        private NumericUpDown nudPrice;
        private NumericUpDown nudStock;
        private ComboBox cboCategory;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;

        public FormProductManagement()
        {
            InitializeComponent();
            this.Load += FormProductManagement_Load;

            // Chỉ Admin và Warehouse được thêm / sửa / xóa
            bool canEdit = SessionManager.CurrentRole == "Admin" || SessionManager.CurrentRole == "Warehouse";
            btnAdd.Enabled = canEdit;
            btnUpdate.Enabled = canEdit;
            btnDelete.Enabled = canEdit;
        }

        // ===== Dựng giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Quản lý Sản phẩm & Kho hàng";
            this.Size = new Size(1100, 600);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 10F);

            // --- Thanh trên: tìm kiếm & lọc ---
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            var lblSearch = new Label { Text = "Mã vạch:", AutoSize = true, Location = new Point(15, 20) };
            txtSearchBarcode = new TextBox { Location = new Point(85, 17), Width = 200 };
            var lblFilter = new Label { Text = "Nhóm hàng:", AutoSize = true, Location = new Point(305, 20) };
            cboFilterCategory = new ComboBox
            {
                Location = new Point(395, 17),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            btnSearch = new Button { Text = "Tìm kiếm", Location = new Point(655, 14), Size = new Size(110, 32) };
            btnSearch.Click += btnSearch_Click;
            panelTop.Controls.AddRange(new Control[] { lblSearch, txtSearchBarcode, lblFilter, cboFilterCategory, btnSearch });

            // --- Bảng sản phẩm (bên trái) ---
            dgvProducts = new DataGridView
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
            dgvProducts.CellClick += dgvProducts_CellClick;

            // --- Khu vực chi tiết (bên phải) ---
            var panelDetail = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                AutoScroll = true
            };

            txtId = new TextBox { ReadOnly = true };
            txtBarcode = new TextBox();
            txtProductName = new TextBox();
            nudPrice = new NumericUpDown
            {
                Maximum = 1000000000,
                DecimalPlaces = 0,
                Increment = 1000,
                ThousandsSeparator = true
            };
            nudStock = new NumericUpDown { Maximum = 1000000, DecimalPlaces = 0 };
            cboCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            AddField(panelDetail, "Mã SP", txtId, 10);
            AddField(panelDetail, "Mã vạch", txtBarcode, 68);
            AddField(panelDetail, "Tên sản phẩm", txtProductName, 126);
            AddField(panelDetail, "Đơn giá (đ)", nudPrice, 184);
            AddField(panelDetail, "Tồn kho", nudStock, 242);
            AddField(panelDetail, "Nhóm hàng", cboCategory, 300);

            btnAdd = CreateButton("Thêm mới", Color.FromArgb(39, 174, 96), 15, 372);
            btnUpdate = CreateButton("Cập nhật", Color.FromArgb(41, 100, 180), 172, 372);
            btnDelete = CreateButton("Xóa", Color.FromArgb(192, 57, 43), 15, 418);
            btnLoad = CreateButton("Tải lại", Color.FromArgb(120, 120, 120), 172, 418);
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnLoad.Click += btnLoad_Click;
            panelDetail.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnLoad });

            // --- Ghép vào Form (thứ tự quyết định cách Dock) ---
            this.Controls.Add(dgvProducts);
            this.Controls.Add(panelDetail);
            this.Controls.Add(panelTop);
            dgvProducts.BringToFront();
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

        private Button CreateButton(string text, Color color, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(148, 38)
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

        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        // Nạp danh sách nhóm hàng vào 2 ComboBox (chọn khi nhập liệu + lọc)
        private async Task LoadCategoriesAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var categories = await client.GetFromJsonAsync<List<CategoryDto>>("categories");
                if (categories == null) return;

                // Mỗi ComboBox dùng một danh sách riêng để chọn ở ô này không làm đổi ô kia
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryId";
                cboCategory.DataSource = new List<CategoryDto>(categories);

                var filterList = new List<CategoryDto>
                {
                    new CategoryDto { CategoryId = 0, CategoryName = "-- Tất cả nhóm hàng --" }
                };
                filterList.AddRange(categories);
                cboFilterCategory.DisplayMember = "CategoryName";
                cboFilterCategory.ValueMember = "CategoryId";
                cboFilterCategory.DataSource = filterList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp nhóm hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nạp sản phẩm, có áp dụng bộ lọc mã vạch / nhóm hàng đang chọn
        private async Task LoadProductsAsync()
        {
            try
            {
                var query = new List<string>();

                string barcode = txtSearchBarcode.Text.Trim();
                if (barcode.Length > 0)
                {
                    query.Add("barcode=" + Uri.EscapeDataString(barcode));
                }

                if (cboFilterCategory.SelectedValue is int catId && catId > 0)
                {
                    query.Add("categoryId=" + catId);
                }

                string url = "products" + (query.Count > 0 ? "?" + string.Join("&", query) : "");

                using var client = GetAuthenticatedClient();
                var products = await client.GetFromJsonAsync<List<ProductDto>>(url);

                dgvProducts.DataSource = products;
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            SetHeader("ProductId", "Mã SP");
            SetHeader("Barcode", "Mã vạch");
            SetHeader("ProductName", "Tên sản phẩm");
            SetHeader("Price", "Đơn giá");
            SetHeader("StockQuantity", "Tồn kho");
            SetHeader("CategoryName", "Nhóm hàng");

            if (dgvProducts.Columns["CategoryId"] != null)
            {
                dgvProducts.Columns["CategoryId"].Visible = false;
            }
            if (dgvProducts.Columns["Price"] != null)
            {
                dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N0";
            }
        }

        private void SetHeader(string columnName, string text)
        {
            var col = dgvProducts.Columns[columnName];
            if (col != null) col.HeaderText = text;
        }

        // Bấm vào dòng nào thì đổ dữ liệu dòng đó sang các ô bên phải
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var p = dgvProducts.Rows[e.RowIndex].DataBoundItem as ProductDto;
            if (p == null) return;

            txtId.Text = p.ProductId.ToString();
            txtBarcode.Text = p.Barcode;
            txtProductName.Text = p.ProductName;
            nudPrice.Value = Math.Min(p.Price, nudPrice.Maximum);
            nudStock.Value = Math.Min(p.StockQuantity, (int)nudStock.Maximum);
            cboCategory.SelectedValue = p.CategoryId;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        // "Tải lại" = xóa bộ lọc rồi nạp lại toàn bộ
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            txtSearchBarcode.Clear();
            if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0;
            await LoadProductsAsync();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã vạch và Tên sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!(cboCategory.SelectedValue is int))
            {
                MessageBox.Show("Vui lòng chọn Nhóm hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                var newProd = new
                {
                    Barcode = txtBarcode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    Price = nudPrice.Value,
                    StockQuantity = (int)nudStock.Value,
                    CategoryId = (int)cboCategory.SelectedValue
                };

                using var client = GetAuthenticatedClient();
                var res = await client.PostAsJsonAsync("products", newProd);

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowFailureAsync(res, "Thêm");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                var updateProd = new
                {
                    ProductId = id,
                    Barcode = txtBarcode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    Price = nudPrice.Value,
                    StockQuantity = (int)nudStock.Value,
                    CategoryId = (int)cboCategory.SelectedValue
                };

                using var client = GetAuthenticatedClient();
                var res = await client.PutAsJsonAsync($"products/{id}", updateProd);

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowFailureAsync(res, "Cập nhật");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận xóa sản phẩm ID = {id}?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.DeleteAsync($"products/{id}");

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã xóa sản phẩm!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else
                {
                    await ShowFailureAsync(res, "Xóa");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị lỗi từ server (403 không có nội dung nên báo riêng)
        private async Task ShowFailureAsync(HttpResponseMessage res, string action)
        {
            if (res.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show($"{action} thất bại: bạn không có quyền thực hiện thao tác này!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string message = await res.Content.ReadAsStringAsync();
            MessageBox.Show($"{action} thất bại!\n\n{message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
        }
    }

    // Dữ liệu sản phẩm nhận từ API (POS ở Phần D cũng dùng lại lớp này)
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}