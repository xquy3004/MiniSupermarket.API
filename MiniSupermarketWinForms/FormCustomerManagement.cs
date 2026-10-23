using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormCustomerManagement : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";
        private const int PageSize = 10;

        // ===== 1. Toolbar & Tìm kiếm =====
        private Panel pnlHeader;
        private Panel pnlSearch;
        private TextBox txtSearch;
        private ComboBox cmbRankFilter;
        private Button btnSearch;

        // ===== 2. Thẻ KPI Tổng quan =====
        private Panel pnlKpi;
        private Label lblKpiTotalVal;
        private Label lblKpiVipVal;
        private Label lblKpiPointsVal;

        // ===== 3. Khung nội dung chính =====
        private Panel pnlContent;
        private Panel pnlLeftCard;
        private Panel pnlCardSpacer;
        private Panel pnlRightCard;

        // Bảng dữ liệu & Phân trang
        private DataGridView dgvCustomers;
        private ContextMenuStrip contextMenuCustomers;
        private ToolStripMenuItem mnuEdit;
        private ToolStripMenuItem mnuAddPoints;
        private ToolStripMenuItem mnuSubPoints;
        private ToolStripMenuItem mnuDelete;

        private Panel pnlPagination;
        private Button btnFirst;
        private Button btnPrev;
        private Label lblPaging;
        private Button btnNext;
        private Button btnLast;

        // Khung thông tin chi tiết (Right Card)
        private Label lblRightTitle;
        private Label lblRightSubtitle;
        private TextBox txtCustomerId;
        private TextBox txtCustomerName;
        private TextBox txtPhoneNumber;
        private TextBox txtAddress;
        private TextBox txtRewardPoints;
        private ComboBox cmbMembershipRank;

        private Button btnSave;
        private Button btnCreate;
        private Button btnRefresh;
        private Button btnDelete;

        // Dữ liệu nội bộ
        private List<CustomerDto> _allCustomers = new List<CustomerDto>();
        private List<CustomerDto> _filteredCustomers = new List<CustomerDto>();
        private int _currentPage = 1;
        private int _totalPages = 1;
        private CustomerDto _selectedCustomer = null;

        public FormCustomerManagement()
        {
            this.KeyPreview = true;
            this.KeyDown += FormCustomerManagement_KeyDown;
            InitializeComponent();
            this.Load += FormCustomerManagement_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Khách hàng";
            this.Size = new Size(1180, 680);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Segoe UI", 9.5F);
            this.Padding = new Padding(12);

            // ==========================================
            // 1. THANH TÌM KIẾM (TOOLBAR)
            // ==========================================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
            };

            pnlSearch = new Panel
            {
                Dock = DockStyle.Left,
                Width = 530,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                Location = new Point(0, 3),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ApplyFilterAndPage(1);
                }
            };

            cmbRankFilter = new ComboBox
            {
                Location = new Point(280, 3),
                Size = new Size(130, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
            cmbRankFilter.Items.AddRange(new object[] { "Tất cả hạng", "Bạch Kim", "Vàng", "Bạc", "Chuẩn" });
            cmbRankFilter.SelectedIndex = 0;
            cmbRankFilter.SelectedIndexChanged += (s, e) => ApplyFilterAndPage(1);

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(420, 2),
                Size = new Size(95, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => ApplyFilterAndPage(1);

            pnlSearch.Controls.AddRange(new Control[] { txtSearch, cmbRankFilter, btnSearch });
            pnlHeader.Controls.Add(pnlSearch);

            // ==========================================
            // 2. THẺ KPI TỔNG QUAN (KHÔNG DÙNG ICON)
            // ==========================================
            pnlKpi = new Panel
            {
                Dock = DockStyle.Top,
                Height = 62,
                Padding = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
            };

            var cardKpi1 = CreateKpiCard("TỔNG KHÁCH HÀNG", "0 khách", "Đang hoạt động trong hệ thống", Color.FromArgb(37, 99, 235), out lblKpiTotalVal);
            cardKpi1.Location = new Point(0, 0);
            cardKpi1.Size = new Size(260, 54);

            var cardKpi2 = CreateKpiCard("KHÁCH VIP & VÀNG", "0 thành viên", "Hạng Bạch Kim & Vàng", Color.FromArgb(124, 58, 237), out lblKpiVipVal);
            cardKpi2.Location = new Point(272, 0);
            cardKpi2.Size = new Size(260, 54);

            var cardKpi3 = CreateKpiCard("TỔNG ĐIỂM TÍCH LŨY", "0 điểm", "Toàn hệ thống Baby Care", Color.FromArgb(217, 119, 6), out lblKpiPointsVal);
            cardKpi3.Location = new Point(544, 0);
            cardKpi3.Size = new Size(260, 54);

            pnlKpi.Controls.AddRange(new Control[] { cardKpi1, cardKpi2, cardKpi3 });

            // ==========================================
            // 3. KHUNG LÀM VIỆC CHÍNH (Card Trái + Card Phải)
            // ==========================================
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // 3.1 Card bên phải: CHI TIẾT & SỬA ĐỔI (Width = 340)
            pnlRightCard = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                Padding = new Padding(14),
                AutoScroll = true
            };
            pnlRightCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlRightCard.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
            };

            lblRightTitle = new Label
            {
                Text = "CHI TIẾT & SỬA ĐỔI",
                Location = new Point(14, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            lblRightSubtitle = new Label
            {
                Text = "Xem và cập nhật thông tin thành viên",
                Location = new Point(14, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            int topY = 66;
            txtCustomerId = new TextBox { ReadOnly = true, BackColor = Color.FromArgb(241, 245, 249), Font = new Font("Segoe UI", 9.5F) };
            txtCustomerName = new TextBox { Font = new Font("Segoe UI", 9.5F) };
            txtPhoneNumber = new TextBox { Font = new Font("Segoe UI", 9.5F) };
            txtAddress = new TextBox { Font = new Font("Segoe UI", 9.5F) };
            txtRewardPoints = new TextBox { Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), TextAlign = HorizontalAlignment.Right };
            cmbMembershipRank = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            cmbMembershipRank.Items.AddRange(new object[] { "Chuẩn", "Bạc", "Vàng", "Bạch Kim" });

            txtRewardPoints.TextChanged += (s, e) => AutoSuggestRankFromPoints();

            topY = AddFieldToCard(pnlRightCard, "Mã khách hàng:", txtCustomerId, topY);
            topY = AddFieldToCard(pnlRightCard, "Họ và tên (*):", txtCustomerName, topY);
            topY = AddFieldToCard(pnlRightCard, "Số điện thoại (*):", txtPhoneNumber, topY);
            topY = AddFieldToCard(pnlRightCard, "Địa chỉ:", txtAddress, topY);
            topY = AddFieldToCard(pnlRightCard, "Điểm tích lũy:", txtRewardPoints, topY);
            topY = AddComboFieldToCard(pnlRightCard, "Hạng thành viên:", cmbMembershipRank, topY);

            // Nút Lưu cập nhật (F3)
            btnSave = new Button
            {
                Text = "LƯU CẬP NHẬT (F3)",
                Location = new Point(14, topY + 8),
                Size = new Size(312, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += btnSave_Click;

            // Nút Thêm mới (F2)
            btnCreate = new Button
            {
                Text = "THÊM MỚI",
                Location = new Point(14, topY + 52),
                Size = new Size(312, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 149, 93),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += btnCreate_Click;

            // Nút Làm mới & Xóa khách hàng
            btnRefresh = new Button
            {
                Text = "Làm mới",
                Location = new Point(14, topY + 96),
                Size = new Size(150, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefresh.Click += async (s, e) =>
            {
                ClearInputs();
                txtSearch.Clear();
                cmbRankFilter.SelectedIndex = 0;
                await LoadDataAsync();
            };

            btnDelete = new Button
            {
                Text = "Xóa khách hàng",
                Location = new Point(176, topY + 96),
                Size = new Size(150, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += btnDelete_Click;

            pnlRightCard.Controls.AddRange(new Control[]
            {
                lblRightTitle, lblRightSubtitle, btnSave, btnCreate, btnRefresh, btnDelete
            });

            // 3.2 Khoảng trống giữa 2 Card
            pnlCardSpacer = new Panel
            {
                Dock = DockStyle.Right,
                Width = 12,
                BackColor = Color.Transparent
            };

            // 3.3 Card bên trái: Bảng dữ liệu + Phân trang (Dock = Fill)
            pnlLeftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(1)
            };
            pnlLeftCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlLeftCard.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
            };

            // Bảng dữ liệu DataGridView (KHÔNG DÙNG MÀU PHÂN HẠNG DÒNG)
            dgvCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                GridColor = Color.FromArgb(241, 245, 249)
            };
            dgvCustomers.RowTemplate.Height = 34;

            dgvCustomers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(241, 245, 249),
                SelectionForeColor = Color.FromArgb(51, 65, 85)
            };
            dgvCustomers.EnableHeadersVisualStyles = false;

            dgvCustomers.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 9F),
                SelectionBackColor = Color.FromArgb(224, 231, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42)
            };

            dgvCustomers.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            dgvCustomers.CellClick += dgvCustomers_CellClick;

            // Context Menu chuột phải (KHÔNG DÙNG ICON)
            contextMenuCustomers = new ContextMenuStrip();
            mnuEdit = new ToolStripMenuItem("Chỉnh sửa thông tin", null, (s, e) => FocusCurrentCustomer());
            mnuAddPoints = new ToolStripMenuItem("Tích +50 điểm thưởng nhanh", null, async (s, e) => await QuickAdjustPointsAsync(50));
            mnuSubPoints = new ToolStripMenuItem("Đổi quà voucher (-50 điểm)", null, async (s, e) => await QuickAdjustPointsAsync(-50));
            mnuDelete = new ToolStripMenuItem("Xóa khách hàng", null, btnDelete_Click);
            contextMenuCustomers.Items.AddRange(new ToolStripItem[] { mnuEdit, new ToolStripSeparator(), mnuAddPoints, mnuSubPoints, new ToolStripSeparator(), mnuDelete });
            dgvCustomers.ContextMenuStrip = contextMenuCustomers;

            // Phân trang
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(8, 6, 8, 6)
            };
            pnlPagination.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, pnlPagination.Width, 0);
                }
            };

            btnFirst = CreatePageButton("<<", 8, 38, (s, e) => ApplyFilterAndPage(1));
            btnPrev = CreatePageButton("<", 50, 32, (s, e) => { if (_currentPage > 1) ApplyFilterAndPage(_currentPage - 1); });

            lblPaging = new Label
            {
                Text = "Trang 1 / 1 (Tổng 0)",
                Location = new Point(90, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            btnNext = CreatePageButton(">", 320, 32, (s, e) => { if (_currentPage < _totalPages) ApplyFilterAndPage(_currentPage + 1); });
            btnLast = CreatePageButton(">>", 356, 38, (s, e) => ApplyFilterAndPage(_totalPages));

            pnlPagination.Controls.AddRange(new Control[] { btnFirst, btnPrev, lblPaging, btnNext, btnLast });

            pnlLeftCard.Controls.Add(dgvCustomers);
            pnlLeftCard.Controls.Add(pnlPagination);

            // Ghép 2 Card vào Content
            pnlContent.Controls.Add(pnlLeftCard);
            pnlContent.Controls.Add(pnlCardSpacer);
            pnlContent.Controls.Add(pnlRightCard);

            // Ghép toàn bộ vào Form
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlKpi);
            this.Controls.Add(pnlHeader);
        }

        private Panel CreateKpiCard(string title, string initialValue, string subtitle, Color accentColor, out Label lblVal)
        {
            var pnl = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(10, 6, 10, 6)
            };
            pnl.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnl.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 4, pnl.Height);
                }
            };

            var lblT = new Label
            {
                Text = title,
                Location = new Point(12, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            lblVal = new Label
            {
                Text = initialValue,
                Location = new Point(12, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            pnl.Controls.AddRange(new Control[] { lblT, lblVal });
            return pnl;
        }

        private int AddFieldToCard(Panel parent, string labelText, TextBox input, int y)
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(14, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            input.Location = new Point(14, y + 18);
            input.Size = new Size(312, 26);

            parent.Controls.Add(lbl);
            parent.Controls.Add(input);
            return y + 50;
        }

        private int AddComboFieldToCard(Panel parent, string labelText, ComboBox combo, int y)
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(14, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            combo.Location = new Point(14, y + 18);
            combo.Size = new Size(312, 26);

            parent.Controls.Add(lbl);
            parent.Controls.Add(combo);
            return y + 50;
        }

        private Button CreatePageButton(string text, int x, int width, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 6),
                Size = new Size(width, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Padding = new Padding(0),
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += onClick;
            return btn;
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

        private async void FormCustomerManagement_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        // ===== XỬ LÝ DỮ LIỆU & PHÂN TRANG =====

        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var customers = await client.GetFromJsonAsync<List<CustomerDto>>("customers");

                _allCustomers = customers ?? new List<CustomerDto>();
                UpdateKpiCards();
                ApplyFilterAndPage(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ API!\n\n" + ex.Message,
                    "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateKpiCards()
        {
            int total = _allCustomers.Count;
            int vipCount = _allCustomers.Count(c => c.MembershipRank == "Bạch Kim" || c.MembershipRank == "Vàng");
            int totalPoints = _allCustomers.Sum(c => c.RewardPoints);

            lblKpiTotalVal.Text = total.ToString("N0") + " khách";
            lblKpiVipVal.Text = vipCount.ToString("N0") + " thành viên";
            lblKpiPointsVal.Text = totalPoints.ToString("N0") + " điểm";
        }

        private void ApplyFilterAndPage(int page)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string rankFilter = cmbRankFilter.SelectedItem?.ToString() ?? "Tất cả hạng";

            var query = _allCustomers.AsEnumerable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => (c.CustomerName != null && c.CustomerName.ToLower().Contains(keyword)) ||
                                         (c.PhoneNumber != null && c.PhoneNumber.Contains(keyword)));
            }

            if (rankFilter != "Tất cả hạng" && rankFilter != "Tất cả")
            {
                query = query.Where(c => c.MembershipRank == rankFilter);
            }

            // Sắp xếp ưu tiên điểm tích lũy cao nhất lên đầu
            _filteredCustomers = query.OrderByDescending(c => c.RewardPoints).ToList();

            _totalPages = (int)Math.Ceiling((double)_filteredCustomers.Count / PageSize);
            if (_totalPages < 1) _totalPages = 1;

            if (page < 1) page = 1;
            if (page > _totalPages) page = _totalPages;
            _currentPage = page;

            var pageItems = _filteredCustomers
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            BindGrid(pageItems);

            lblPaging.Text = $"Trang {_currentPage} / {_totalPages}  (Tổng: {_filteredCustomers.Count} khách)";
            btnFirst.Enabled = btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = btnLast.Enabled = _currentPage < _totalPages;

            // Di chuyển nút next/last sát bên nhãn paging
            int nextX = lblPaging.Right + 12;
            btnNext.Location = new Point(nextX, 6);
            btnLast.Location = new Point(nextX + 36, 6);

            if (pageItems.Count > 0)
            {
                SelectCustomer(pageItems[0]);
            }
            else
            {
                ClearInputs();
            }
        }

        private void BindGrid(List<CustomerDto> list)
        {
            dgvCustomers.DataSource = null;
            dgvCustomers.Columns.Clear();

            dgvCustomers.AutoGenerateColumns = false;

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerId",
                HeaderText = "Mã KH",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerName",
                HeaderText = "Họ và tên khách hàng",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PhoneNumber",
                HeaderText = "Số điện thoại",
                Width = 115,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Address",
                HeaderText = "Địa chỉ",
                Width = 170
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RewardPoints",
                HeaderText = "Điểm tích lũy",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MembershipRank",
                HeaderText = "Hạng thành viên",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8.75F, FontStyle.Bold)
                }
            });

            dgvCustomers.DataSource = list;
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;

            var row = dgvCustomers.Rows[e.RowIndex];
            var customer = row.DataBoundItem as CustomerDto;
            if (customer != null)
            {
                SelectCustomer(customer);
            }
        }

        private void SelectCustomer(CustomerDto customer)
        {
            _selectedCustomer = customer;
            txtCustomerId.Text = customer.CustomerId.ToString();
            txtCustomerName.Text = customer.CustomerName ?? "";
            txtPhoneNumber.Text = customer.PhoneNumber ?? "";
            txtAddress.Text = customer.Address ?? "";
            txtRewardPoints.Text = customer.RewardPoints.ToString("N0");

            int idx = cmbMembershipRank.FindStringExact(customer.MembershipRank ?? "Chuẩn");
            cmbMembershipRank.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void FocusCurrentCustomer()
        {
            if (dgvCustomers.CurrentRow != null)
            {
                var c = dgvCustomers.CurrentRow.DataBoundItem as CustomerDto;
                if (c != null)
                {
                    SelectCustomer(c);
                    txtCustomerName.Focus();
                }
            }
        }

        private void PrepareNewCustomer()
        {
            ClearInputs();
            txtCustomerId.Text = "(Tự động cấp)";
            txtRewardPoints.Text = "0";
            cmbMembershipRank.SelectedIndex = 0; // Chuẩn
            txtCustomerName.Focus();
        }

        private void ClearInputs()
        {
            _selectedCustomer = null;
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            txtRewardPoints.Text = "0";
            cmbMembershipRank.SelectedIndex = 0;
        }

        // Tự động gợi ý hạng thẻ theo điểm tích lũy
        private void AutoSuggestRankFromPoints()
        {
            if (int.TryParse(txtRewardPoints.Text.Replace(",", "").Replace(".", "").Trim(), out int points))
            {
                string rank = "Chuẩn";
                if (points >= 400) rank = "Bạch Kim";
                else if (points >= 200) rank = "Vàng";
                else if (points >= 50) rank = "Bạc";

                int idx = cmbMembershipRank.FindStringExact(rank);
                if (idx >= 0 && cmbMembershipRank.Focused == false)
                {
                    cmbMembershipRank.SelectedIndex = idx;
                }
            }
        }

        // ===== THỰC HIỆN THÊM / SỬA / XÓA =====

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            // Nếu đang xem một khách hàng có sẵn trên bảng, bấm THÊM MỚI sẽ chuyển sang form nhập mới
            if (_selectedCustomer != null && int.TryParse(txtCustomerId.Text, out int existingId) && existingId > 0)
            {
                PrepareNewCustomer();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ tên và Số điện thoại khách hàng!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return;
            }

            int points = 0;
            int.TryParse(txtRewardPoints.Text.Replace(",", "").Replace(".", "").Trim(), out points);

            var newCus = new
            {
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = points,
                MembershipRank = cmbMembershipRank.SelectedItem?.ToString() ?? "Chuẩn"
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("customers", newCus);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm khách hàng mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string msg = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Thêm khách hàng thất bại!\n\n" + msg, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi gọi API:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCustomerId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng trên bảng trước khi lưu cập nhật!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Họ tên và Số điện thoại không được để trống!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int points = 0;
            int.TryParse(txtRewardPoints.Text.Replace(",", "").Replace(".", "").Trim(), out points);

            var updateCus = new
            {
                CustomerId = id,
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = points,
                MembershipRank = cmbMembershipRank.SelectedItem?.ToString() ?? "Chuẩn"
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"customers/{id}", updateCus);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string msg = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Cập nhật thất bại!\n\n" + msg, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!int.TryParse(txtCustomerId.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa khách hàng [{txtCustomerName.Text}] (ID = {id})?\nThao tác này không thể hoàn tác!",
                "Xác nhận xóa khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"customers/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã xóa khách hàng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else
                {
                    string msg = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Xóa khách hàng thất bại!\n\n" + msg, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tích điểm / Đổi thưởng nhanh từ menu chuột phải
        private async Task QuickAdjustPointsAsync(int deltaPoints)
        {
            if (dgvCustomers.CurrentRow == null) return;
            var c = dgvCustomers.CurrentRow.DataBoundItem as CustomerDto;
            if (c == null) return;

            int newPoints = Math.Max(0, c.RewardPoints + deltaPoints);
            string actionName = deltaPoints > 0 ? "cộng" : "đổi thưởng/trừ";

            string rank = "Chuẩn";
            if (newPoints >= 400) rank = "Bạch Kim";
            else if (newPoints >= 200) rank = "Vàng";
            else if (newPoints >= 50) rank = "Bạc";

            var updateCus = new
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                RewardPoints = newPoints,
                MembershipRank = rank
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"customers/{c.CustomerId}", updateCus);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Đã {actionName} {Math.Abs(deltaPoints)} điểm cho khách hàng {c.CustomerName}!\nĐiểm mới: {newPoints} điểm (Hạng: {rank})",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật điểm:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phím tắt bàn phím tiện ích
        private void FormCustomerManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                PrepareNewCustomer();
            }
            else if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                btnSave.PerformClick();
            }
            else if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                btnRefresh.PerformClick();
            }
            else if (e.KeyCode == Keys.Delete && !txtCustomerName.Focused && !txtPhoneNumber.Focused && !txtAddress.Focused && !txtSearch.Focused)
            {
                e.Handled = true;
                btnDelete.PerformClick();
            }
        }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = string.Empty;
    }
}
