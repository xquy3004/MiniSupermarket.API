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
                [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);

        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // Cấu trúc bố cục chính
        private Panel pnlContent;
        private Panel pnlLeftCard;
        private Panel pnlCardSpacer;
        private Panel pnlRightCard;

        // Vùng trên bên trái: Tiêu đề + Tìm kiếm / Lọc / Thao tác nhanh
        private Panel pnlLeftTop;
        private TextBox txtKeyword;
        private ComboBox cboFilterCategory;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnLoad;

        // Bảng dữ liệu sản phẩm
        private DataGridView dgvProducts;
        private ContextMenuStrip contextMenuProducts;
        private ToolStripMenuItem mnuViewDetail;
        private ToolStripMenuItem mnuEditProduct;
        private ToolStripMenuItem mnuDeleteProduct;

        // Vùng dưới bên trái: Thông tin chọn + Điều hướng phân trang
        private Panel pnlLeftBottom;
        private Label lblSelectionInfo;
        private Panel pnlPaging;
        private Button btnFirst;
        private Button btnPrev;
        private Label lblPaging;
        private Button btnNext;
        private Button btnLast;

        // Vùng chi tiết bên phải: CHI TIẾT & SỬA ĐỔI
        private Label lblDetailTitle;
        private Label lblId;
        private TextBox txtId;
        private Label lblBarcode;
        private TextBox txtBarcode;
        private Label lblProductName;
        private TextBox txtProductName;
        private Label lblCategory;
        private ComboBox cboCategory;
        private Label lblPrice;
        private NumericUpDown nudPrice;
        private Label lblStock;
        private NumericUpDown nudStock;
        private Panel pnlActionButtons;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSave;

        // Quản lý trạng thái phân trang (10 sản phẩm / trang)
        private int _currentPage = 1;
        private readonly int _pageSize = 10;
        private int _totalPages = 1;
        private int _totalItems = 0;
        private string _currentKeyword = string.Empty;
        private int? _currentCategoryId = null;

        public FormProductManagement()
        {
            InitializeComponent();
            this.Load += FormProductManagement_Load;
            this.KeyDown += FormProductManagement_KeyDown;
        }

        private void InitializeComponent()
        {
            this.Text = "QUẢN LÝ SẢN PHẨM";
            this.ClientSize = new Size(1100, 625);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ==========================================
            // 1. Panel Content bao bọc toàn bộ form
            // ==========================================
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 12, 16, 16),
                BackColor = Color.FromArgb(244, 245, 247)
            };

            // ==========================================
            // 2. Card bên phải: CHI TIẾT & SỬA ĐỔI
            // ==========================================
            pnlRightCard = new Panel
            {
                Dock = DockStyle.Right,
                Width = 310,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 12),
                AutoScroll = true
            };

            lblDetailTitle = new Label
            {
                Text = "CHI TIẾT & SỬA ĐỔI",
                UseMnemonic = false,
                Location = new Point(16, 12),
                Size = new Size(275, 24),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            lblId = new Label { Text = "Mã sản phẩm:", AutoSize = true, Location = new Point(16, 40), ForeColor = Color.FromArgb(71, 85, 105) };
            txtId = new TextBox
            {
                Location = new Point(16, 58),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                BackColor = Color.FromArgb(241, 245, 249),
                Font = new Font("Segoe UI", 9.5F)
            };

            lblBarcode = new Label { Text = "Mã vạch:", AutoSize = true, Location = new Point(16, 86), ForeColor = Color.FromArgb(71, 85, 105) };
            txtBarcode = new TextBox
            {
                Location = new Point(16, 104),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5F)
            };

            lblProductName = new Label { Text = "Tên sản phẩm:", AutoSize = true, Location = new Point(16, 132), ForeColor = Color.FromArgb(71, 85, 105) };
            txtProductName = new TextBox
            {
                Location = new Point(16, 150),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5F)
            };

            lblCategory = new Label { Text = "Nhóm hàng:", AutoSize = true, Location = new Point(16, 178), ForeColor = Color.FromArgb(71, 85, 105) };
            cboCategory = new ComboBox
            {
                Location = new Point(16, 196),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };

            lblPrice = new Label { Text = "Đơn giá (VNĐ):", AutoSize = true, Location = new Point(16, 224), ForeColor = Color.FromArgb(71, 85, 105) };
            nudPrice = new NumericUpDown
            {
                Location = new Point(16, 242),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Maximum = 1000000000,
                DecimalPlaces = 0,
                Increment = 1000,
                ThousandsSeparator = true,
                Font = new Font("Segoe UI", 9.5F)
            };

            lblStock = new Label { Text = "Số lượng tồn kho:", AutoSize = true, Location = new Point(16, 270), ForeColor = Color.FromArgb(71, 85, 105) };
            nudStock = new NumericUpDown
            {
                Location = new Point(16, 288),
                Size = new Size(275, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Maximum = 1000000,
                DecimalPlaces = 0,
                Font = new Font("Segoe UI", 9.5F)
            };

            pnlActionButtons = new Panel
            {
                Location = new Point(16, 322),
                Size = new Size(275, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnUpdate = new Button
            {
                Text = "CẬP NHẬT",
                Location = new Point(0, 2),
                Size = new Size(132, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += btnUpdate_Click;

            btnDelete = new Button
            {
                Text = "XÓA",
                Location = new Point(143, 2),
                Size = new Size(132, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += btnDelete_Click;

            pnlActionButtons.Controls.Add(btnUpdate);
            pnlActionButtons.Controls.Add(btnDelete);

            btnSave = new Button
            {
                Text = "LƯU THAY ĐỔI",
                Location = new Point(16, 368),
                Size = new Size(275, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += btnSave_Click;

            pnlRightCard.Controls.AddRange(new Control[]
            {
                lblDetailTitle, lblId, txtId, lblBarcode, txtBarcode,
                lblProductName, txtProductName, lblCategory, cboCategory,
                lblPrice, nudPrice, lblStock, nudStock, pnlActionButtons, btnSave
            });

            // ==========================================
            // 3. Khoảng cách giữa 2 Card
            // ==========================================
            pnlCardSpacer = new Panel
            {
                Dock = DockStyle.Right,
                Width = 14
            };

            // ==========================================
            // 4. Card bên trái: Bảng danh sách sản phẩm
            // ==========================================
            pnlLeftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14)
            };

            // 4.1 Thanh tìm kiếm & lọc trên cùng của Left Card (Bố cục dàn đều 2 bên, chống tràn)
            pnlLeftTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(0, 0, 0, 8)
            };

            var pnlActionsRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 232,
                Height = 36
            };

            btnAdd = new Button
            {
                Text = "+ THÊM MỚI (F2)",
                Location = new Point(0, 2),
                Size = new Size(122, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += btnAdd_Click;

            btnLoad = new Button
            {
                Text = "LÀM MỚI (F5)",
                Location = new Point(128, 2),
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += btnLoad_Click;

            pnlActionsRight.Controls.AddRange(new Control[] { btnAdd, btnLoad });

            var pnlSearchLeft = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 36
            };

            txtKeyword = new TextBox
            {
                Location = new Point(0, 4),
                Size = new Size(130, 24),
                Font = new Font("Segoe UI", 9.5F)
            };
            txtKeyword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.PerformClick();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            cboFilterCategory = new ComboBox
            {
                Location = new Point(136, 4),
                Size = new Size(140, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };

            btnSearch = new Button
            {
                Text = "🔍",
                Location = new Point(282, 3),
                Size = new Size(36, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += btnSearch_Click;

            pnlSearchLeft.Controls.AddRange(new Control[] { txtKeyword, cboFilterCategory, btnSearch });

            pnlLeftTop.Controls.Add(pnlSearchLeft);
            pnlLeftTop.Controls.Add(pnlActionsRight);

            // 4.2 DataGridView sản phẩm
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
                BorderStyle = BorderStyle.Fixed3D,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36
            };
            dgvProducts.RowTemplate.Height = 32;

            dgvProducts.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(241, 245, 249),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(6, 0, 6, 0),
                SelectionBackColor = Color.FromArgb(241, 245, 249),
                SelectionForeColor = Color.FromArgb(30, 41, 59)
            };

            dgvProducts.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(4),
                SelectionBackColor = Color.FromArgb(219, 234, 254),
                SelectionForeColor = Color.FromArgb(30, 41, 59)
            };

            dgvProducts.CellClick += dgvProducts_CellClick;

            // Context Menu chuột phải
            contextMenuProducts = new ContextMenuStrip();
            mnuViewDetail = new ToolStripMenuItem("Xem chi tiết", null, (s, e) => FocusCurrentRow());
            mnuEditProduct = new ToolStripMenuItem("Sửa sản phẩm", null, (s, e) => { FocusCurrentRow(); txtProductName.Focus(); txtProductName.SelectAll(); });
            mnuDeleteProduct = new ToolStripMenuItem("Xóa sản phẩm", null, btnDelete_Click);
            contextMenuProducts.Items.AddRange(new ToolStripItem[] { mnuViewDetail, mnuEditProduct, mnuDeleteProduct });
            dgvProducts.ContextMenuStrip = contextMenuProducts;

            // 4.3 Vùng phân trang và thông tin dòng chọn
            pnlLeftBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 38,
                Padding = new Padding(4, 2, 4, 2)
            };

            lblSelectionInfo = new Label
            {
                Text = "Thông tin sản phẩm đang chọn: (Chưa chọn)",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(4, 10)
            };

            pnlPaging = new Panel
            {
                Dock = DockStyle.Right,
                Width = 330,
                Height = 34
            };

            btnFirst = new Button
            {
                Text = "<<",
                Location = new Point(0, 3),
                Size = new Size(34, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFirst.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnFirst.Click += btnFirst_Click;

            btnPrev = new Button
            {
                Text = "<",
                Location = new Point(38, 3),
                Size = new Size(32, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrev.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrev.Click += btnPrev_Click;

            lblPaging = new Label
            {
                Text = "Trang 1 / 1  (Tổng: 0 SP)",
                Location = new Point(74, 3),
                Size = new Size(180, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            btnNext = new Button
            {
                Text = ">",
                Location = new Point(258, 3),
                Size = new Size(32, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnNext.Click += btnNext_Click;

            btnLast = new Button
            {
                Text = ">>",
                Location = new Point(294, 3),
                Size = new Size(34, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLast.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnLast.Click += btnLast_Click;

            pnlPaging.Controls.AddRange(new Control[] { btnFirst, btnPrev, lblPaging, btnNext, btnLast });
            pnlLeftBottom.Controls.AddRange(new Control[] { pnlPaging, lblSelectionInfo });

            pnlLeftCard.Controls.Add(dgvProducts);
            pnlLeftCard.Controls.Add(pnlLeftBottom);
            pnlLeftCard.Controls.Add(pnlLeftTop);

            // ==========================================
            // 5. Ghép các Panel vào pnlContent và Form
            // ==========================================
            pnlContent.Controls.Add(pnlLeftCard);
            pnlContent.Controls.Add(pnlCardSpacer);
            pnlContent.Controls.Add(pnlRightCard);

            this.Controls.Add(pnlContent);
        }

        // ===== Phím tắt bàn phím F2 & F5 =====
        private void FormProductManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                PrepareAddNew();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                _ = ReloadAllAsync();
                e.Handled = true;
            }
        }

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
            ApplyRolePermissions();
            try { SendMessage(txtKeyword.Handle, 0x1501, 1, "Tìm tên hoặc mã vạch..."); } catch { }
            await LoadCategoriesAsync();
            await LoadProductsAsync(1);
        }

        private void ApplyRolePermissions()
        {
            bool canEdit = SessionManager.CurrentRole == "Admin" || SessionManager.CurrentRole == "Warehouse";
            btnAdd.Enabled = canEdit;
            btnUpdate.Enabled = canEdit;
            btnDelete.Enabled = canEdit;
            btnSave.Enabled = canEdit;
        }

        // Nạp danh mục vào 2 ComboBox
        private async Task LoadCategoriesAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var categories = await client.GetFromJsonAsync<List<CategoryDto>>("categories");
                if (categories == null) return;

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

        // Nạp sản phẩm phân trang
        private async Task LoadProductsAsync(int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.Append($"products/paged?pageNumber={pageNumber}&pageSize={_pageSize}");

                if (!string.IsNullOrWhiteSpace(_currentKeyword))
                {
                    sb.Append($"&keyword={Uri.EscapeDataString(_currentKeyword)}");
                }

                if (_currentCategoryId.HasValue && _currentCategoryId.Value > 0)
                {
                    sb.Append($"&categoryId={_currentCategoryId.Value}");
                }

                using var client = GetAuthenticatedClient();
                var result = await client.GetFromJsonAsync<PagedViewResult<ProductItemViewDto>>(sb.ToString());

                if (result != null)
                {
                    _currentPage = result.CurrentPage;
                    _totalPages = Math.Max(1, result.TotalPages);
                    _totalItems = result.TotalItems;

                    dgvProducts.DataSource = result.Items;
                    FormatGrid();

                    lblPaging.Text = $"Trang {_currentPage} / {_totalPages}  (Tổng: {_totalItems} SP)";
                    btnFirst.Enabled = _currentPage > 1;
                    btnPrev.Enabled = _currentPage > 1;
                    btnNext.Enabled = _currentPage < _totalPages;
                    btnLast.Enabled = _currentPage < _totalPages;

                    if (dgvProducts.Rows.Count > 0)
                    {
                        SelectRow(0);
                    }
                    else
                    {
                        ClearInputs();
                        lblSelectionInfo.Text = string.IsNullOrWhiteSpace(_currentKeyword)
                            ? "Thông tin sản phẩm đang chọn: (Chưa có dữ liệu)"
                            : $"Không tìm thấy sản phẩm phù hợp với từ khóa '{_currentKeyword}'";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            SetHeader("ProductId", "Mã SP", 10);
            SetHeader("Barcode", "Mã vạch", 16);
            SetHeader("ProductName", "Tên sản phẩm", 30);
            SetHeader("CategoryName", "Nhóm hàng", 18);
            SetHeader("Price", "Đơn giá (đ)", 14);
            SetHeader("StockQuantity", "Tồn kho", 12);

            if (dgvProducts.Columns["CategoryId"] != null)
                dgvProducts.Columns["CategoryId"].Visible = false;

            if (dgvProducts.Columns["IsLowStock"] != null)
                dgvProducts.Columns["IsLowStock"].Visible = false;

            if (dgvProducts.Columns["Price"] != null)
            {
                dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N0";
                dgvProducts.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProducts.Columns["Price"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvProducts.Columns["StockQuantity"] != null)
            {
                dgvProducts.Columns["StockQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProducts.Columns["StockQuantity"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void SetHeader(string colName, string text, float weight)
        {
            var col = dgvProducts.Columns[colName];
            if (col != null)
            {
                col.HeaderText = text;
                col.FillWeight = weight;
            }
        }

        private void SelectRow(int index)
        {
            if (index >= 0 && index < dgvProducts.Rows.Count)
            {
                dgvProducts.ClearSelection();
                dgvProducts.Rows[index].Selected = true;
                var p = dgvProducts.Rows[index].DataBoundItem as ProductItemViewDto;
                if (p != null) FillDetails(p);
            }
        }

        private void FillDetails(ProductItemViewDto p)
        {
            txtId.Text = p.ProductId.ToString();
            txtBarcode.Text = p.Barcode;
            txtProductName.Text = p.ProductName;
            nudPrice.Value = Math.Min(p.Price, nudPrice.Maximum);
            nudStock.Value = Math.Min(p.StockQuantity, (int)nudStock.Maximum);
            if (p.CategoryId > 0)
                cboCategory.SelectedValue = p.CategoryId;

            lblSelectionInfo.Text = $"Thông tin sản phẩm đang chọn ({p.ProductId})";
        }

        private void FocusCurrentRow()
        {
            if (dgvProducts.CurrentRow != null)
            {
                var p = dgvProducts.CurrentRow.DataBoundItem as ProductItemViewDto;
                if (p != null) FillDetails(p);
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvProducts.Rows.Count)
            {
                var p = dgvProducts.Rows[e.RowIndex].DataBoundItem as ProductItemViewDto;
                if (p != null) FillDetails(p);
            }
        }

        private void PrepareAddNew()
        {
            ClearInputs();
            lblSelectionInfo.Text = "Thông tin sản phẩm đang chọn: [Đang thêm mới]";
            txtBarcode.Focus();
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
        }

        private async Task ReloadAllAsync()
        {
            txtKeyword.Clear();
            _currentKeyword = string.Empty;
            if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0;
            _currentCategoryId = null;
            await LoadProductsAsync(1);
        }

        // ==========================================
        // Sự kiện các nút chức năng
        // ==========================================
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _currentKeyword = txtKeyword.Text.Trim();
            _currentCategoryId = cboFilterCategory.SelectedValue is int c && c > 0 ? c : (int?)null;
            await LoadProductsAsync(1);
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await ReloadAllAsync();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PrepareAddNew();
        }

        private async void btnFirst_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1) await LoadProductsAsync(1);
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1) await LoadProductsAsync(_currentPage - 1);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages) await LoadProductsAsync(_currentPage + 1);
        }

        private async void btnLast_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages) await LoadProductsAsync(_totalPages);
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã vạch và Tên sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!(cboCategory.SelectedValue is int c && c > 0))
            {
                MessageBox.Show("Vui lòng chọn Nhóm hàng hợp lệ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtId.Text, out int id) && id > 0)
            {
                await ExecuteUpdateAsync(id);
            }
            else
            {
                await ExecuteAddAsync();
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật từ bảng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await ExecuteUpdateAsync(id);
        }

        private async Task ExecuteUpdateAsync(int id)
        {
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
                    MessageBox.Show("Cập nhật thông tin sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync(_currentPage);
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

        private async void btnAddAction_Click(object sender, EventArgs e)
        {
            await ExecuteAddAsync();
        }

        private async Task ExecuteAddAsync()
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
                    await LoadProductsAsync(_currentPage);
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

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa từ bảng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận xóa sản phẩm [{txtProductName.Text}] (Mã SP: {id})?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.DeleteAsync($"products/{id}");

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dgvProducts.Rows.Count <= 1 && _currentPage > 1)
                        _currentPage--;
                    await LoadProductsAsync(_currentPage);
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

        private async Task ShowFailureAsync(HttpResponseMessage res, string action)
        {
            if (res.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show($"{action} thất bại: Bạn không có quyền thực hiện thao tác này!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string message = await res.Content.ReadAsStringAsync();
            MessageBox.Show($"{action} thất bại!\n\n{message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // DTO sản phẩm dùng cho toàn bộ hệ thống
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

    public class ProductItemViewDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsLowStock { get; set; }
    }

    public class PagedViewResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public System.Collections.Generic.List<T> Items { get; set; } = new System.Collections.Generic.List<T>();
    }
}
